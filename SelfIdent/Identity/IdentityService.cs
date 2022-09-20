using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Account.Authentication;
using SelfIdent.DatabaseServices;
using SelfIdent.Exceptions;
using SelfIdent.Interfaces;
using SelfIdent.Options;
using SelfIdent.Cryptography;
using SelfIdent.Account.Registration;
using SelfIdent.Identity.Results;
using SelfIdent.Account.Update;
using SelfIdent.Account.Deletion;
using SelfIdent.Account.Lock;
using SelfIdent.Helpers;
using SelfIdent.Enums;
using SelfIdent.Account.SecurityContextInvalidation;

namespace SelfIdent.Identity;

internal class IdentityService : IIdentityService
{
    private IDatabaseService _databaseService { get; set; }
    private ICryptographyService _cryptoService { get; set; }
    private SelfIdentOptions _options { get; set; }

    public IdentityService(IDatabaseService databaseService, ICryptographyService cryptoService, SelfIdentOptions options)
    {
        _databaseService = databaseService;
        _cryptoService = cryptoService;
        _options = options;
    }

    public IdentityCreationResult CreateIdentity(RegistrationPayload payload)
    {
        //Assemble Information for Database and pass it to the DatabaseService
        var result = new IdentityCreationResult();

        try
        {
            PasswordHashResult passwordResult = _cryptoService.HashPassword(payload.Password);

            var newIdentity = new UserIdentity(
                                                payload.Username,
                                                payload.Email,
                                                passwordResult.Hash,
                                                passwordResult.Salt
                                                );

            newIdentity.HashingOptions = passwordResult.HashingOptions;
            newIdentity.HashFunctionType = _options.HashFunctionType;

            if (payload.Roles == null || payload.Roles.Count == 0 || !RolesAreDefined(payload.Roles))
                newIdentity.Roles = _options.RoleOptions.DefaultRoles;
            else
                newIdentity.Roles = payload.Roles;

            if (_databaseService.IdentityExists(newIdentity))
                throw new UserOrEmailAlreadyInUseException();

            if (_options.LockAccountOnRegistration)
                LockUserIdentity(newIdentity);

            IdentityDatabaseResult databaseResult = _databaseService.InsertIdentity(newIdentity);

            if (!databaseResult.Successful)
            {
                result.ThrownException = databaseResult.ThrownException;
                result.Successful = false;
                result.CreatedIdentity = null;

                return result;
            }

            result.CreatedIdentity = databaseResult.Identity;

            return result;
        }
        catch (Exception e)
        {
            result.ThrownException = e;
            result.Successful = false;

            return result;
        }
    }

    private bool RolesAreDefined(List<Roles.Role>? roles)
    {
        if (roles == null || roles.Count == 0)
            return false;

        foreach (Roles.Role role in roles)
        {
            if (_options.RoleOptions.Roles.FirstOrDefault(r => r.Name == role.Name && r.ValueKey == role.ValueKey) == null)
                return false;
        }

        return true;
    }

    public IdentityUpdateResult UpdateIdentity(UpdatePayload payload)
    {
        var result = new IdentityUpdateResult();

        try
        {
            if (payload.Id == 0)
                throw new ArgumentException("No ID given.");

            IdentityDatabaseResult getResult = _databaseService.GetIdentity(payload.Id);

            UserIdentity existingIdentity = Helper.CheckDatabaseResult<UserIdentity>(getResult, new Exception("Failed getting existing Identity"));
            UserIdentity updatedIdentity = existingIdentity.Clone();

            // Only update if a new value is given
            if (!String.IsNullOrEmpty(payload.Username))
                updatedIdentity.Name = payload.Username;

            if (!String.IsNullOrEmpty(payload.Email))
                updatedIdentity.Email = payload.Email;

            if (!String.IsNullOrEmpty(payload.Password))
            {
                // New Password was given. 
                // Hash it and update the identity with the current hashing options
                PasswordHashResult passwordResult = _cryptoService.HashPassword(payload.Password);

                updatedIdentity.Hash = passwordResult.Hash;
                updatedIdentity.Salt = passwordResult.Salt;
                updatedIdentity.HashingOptions = passwordResult.HashingOptions;
            }

            if (payload.Roles != null && payload.Roles.Count > 0 && !Roles.Role.RolesMatch(payload.Roles, updatedIdentity.Roles) && RolesAreDefined(payload.Roles))
            {
                IdentityDatabaseResult deleteRolesResult = _databaseService.DeleteIdentityRoles(updatedIdentity);
                Helper.CheckDatabaseResult<UserIdentity>(deleteRolesResult, new Exception("Error deleting existing role assignments"));

                updatedIdentity.Roles = payload.Roles;

                foreach (Roles.Role role in updatedIdentity.Roles)
                {
                    IdentityDatabaseResult insertRoleResult = _databaseService.InsertIdentityRole(updatedIdentity.Id, role);
                    Helper.CheckDatabaseResult<UserIdentity>(insertRoleResult, new Exception("Error inserting new role assignments"));
                }
            }

            // Identity has been updated. 
            // Check if SecurityContext for this Identity has been invalidated by it.
            updatedIdentity.IntegrityStatus = GetIntegrityStatusFromDifferences(updatedIdentity, existingIdentity, _options.InvalidationStrictness);

            IdentityDatabaseResult updateResult = _databaseService.UpdateIdentity(updatedIdentity);
            Helper.CheckDatabaseResult<UserIdentity>(updateResult, new Exception("Error updating Identity"));

            result.UpdatedIdentity = updatedIdentity;
            result.Successful = true;

            return result;
        }
        catch (Exception e)
        {
            result.ThrownException = e;
            result.Successful = false;

            return result;
        }
    }

    private SecurityContextIntegrityStatus GetIntegrityStatusFromDifferences(UserIdentity newIdentity, UserIdentity oldIdentity, SecurityContextInvalidationStrictness strictness)
    {
        if (oldIdentity.IntegrityStatus == SecurityContextIntegrityStatus.INVALID)
            return SecurityContextIntegrityStatus.INVALID;

        List<string> properties = GetPropertiesWithDifference(newIdentity, oldIdentity);
        SecurityContextIntegrityStatus result = oldIdentity.IntegrityStatus;

        if (properties.Any())
            result = GetMostStrictIntegrityStatus(result, SecurityContextIntegrityStatus.UPDATE);

        switch (strictness)
        {
            case SecurityContextInvalidationStrictness.Never:
                return result;
            case SecurityContextInvalidationStrictness.PasswordChange:
                // If Password changed => Invalidate
                if (properties.Contains(nameof(newIdentity.Hash)))
                    result = SecurityContextIntegrityStatus.INVALID;
                break;
            case SecurityContextInvalidationStrictness.KeyInformationChange:

                // If Key-Properties changed => Invalidate
                if (properties.Contains(nameof(newIdentity.Name)) ||
                    properties.Contains(nameof(newIdentity.Hash)) ||
                    properties.Contains(nameof(newIdentity.Email)))
                    result = SecurityContextIntegrityStatus.INVALID;
                break;
            case SecurityContextInvalidationStrictness.Always:
                // If any of the important Properties are changed => Invalidate
                if (properties.Any())
                    result = SecurityContextIntegrityStatus.INVALID;
                break;
            default:
                throw new NotImplementedException();
        }

        return result;
    }

    private List<string> GetPropertiesWithDifference(UserIdentity first, UserIdentity second)
    {
        //Excludes: AccountData. - LoginAttempts, RegistrationTime, LastLogin
        //        : HashingOptions. -
        //        : HashFunctionType

        var properties = new List<string>();

        AddPropertyIfValueMismatch(first.Salt, second.Salt, nameof(second.Salt), properties);
        AddPropertyIfValueMismatch(first.Name, second.Name, nameof(second.Name), properties);
        AddPropertyIfValueMismatch(first.Email, second.Email, nameof(second.Email), properties);
        AddPropertyIfValueMismatch(first.Hash, second.Hash, nameof(second.Hash), properties);
        AddPropertyIfValueMismatch(first.Id, second.Id, nameof(second.Id), properties);
        AddPropertyIfValueMismatch(first.AccountData.Locked, second.AccountData.Locked, nameof(second.AccountData.Locked), properties);
        AddPropertyIfValueMismatch(first.AccountData.LockKey, second.AccountData.LockKey, nameof(second.AccountData.LockKey), properties);

        if (!Roles.Role.RolesMatch(first.Roles, second.Roles))
            properties.Add(nameof(first.Roles));

        return properties;
    }

    private void AddPropertyIfValueMismatch(object? first, object? second, string name, List<string> properties)
    {
        if (first != second)
            properties.Add(name);
    }

    private SecurityContextIntegrityStatus GetMostStrictIntegrityStatus(SecurityContextIntegrityStatus first, SecurityContextIntegrityStatus second)
    {
        return (SecurityContextIntegrityStatus)Math.Max((int)first, (int)second);
    }

    public IdentityDeletionResult DeleteIdentity(DeletionPayload payload)
    {
        var result = new IdentityDeletionResult();

        try
        {
            IdentityDatabaseResult getResult = _databaseService.GetIdentity(payload.Id);

            Helper.CheckDatabaseResult<UserIdentity>(getResult, new Exception("Issue getting Identity to delete."));

            if (getResult.Identity == null)
                throw new CriticalStateException("Tried deleting a user that does not exist.");

            result.Successful = true;
            result.DeletedIdentity = getResult.Identity;

            IdentityDatabaseResult deleteResult = _databaseService.DeleteIdentity(getResult.Identity);

            Helper.CheckDatabaseResult<UserIdentity>(deleteResult, new Exception("Issue deleting Identity."));

            return result;
        }
        catch (Exception e)
        {
            result.ThrownException = e;
            result.Successful = false;

            return result;
        }
    }

    public IdentityAuthenticationResult AuthenticateIdentity(AuthenticationPayload payload)
    {
        var result = new IdentityAuthenticationResult();
        result.Successful = true;

        try
        {
            IdentityDatabaseResult getResult = _databaseService.GetIdentity(payload.Username, payload.Email);

            if (!getResult.Successful && getResult.Identity == null)
            {
                result.UserExists = false;

                return result;
            }

            UserIdentity identity = Helper.CheckDatabaseResult<UserIdentity>(getResult, new Exception("Error fetching User."));

            bool authenticationResult = _cryptoService.MatchPasswords(payload.Password, identity.Salt, identity.Hash, identity.HashingOptions);

            result.AuthenticatedIdentity = identity;
            result.UserExists = true;
            result.AuthenticatedIdentity.AuthenticationData.Authenticated = authenticationResult;

            if (authenticationResult)
            {
                // If Authentication succeeds => Update with current Timestamp as LastLogin
                identity.AccountData.LastLogin = DateTime.Now;
                identity.IntegrityStatus = SecurityContextIntegrityStatus.VALID;

                UpdateAuthenticatedUser(identity, payload);
            }

            return result;
        }
        catch (Exception e)
        {
            result.ThrownException = e;
            result.Successful = false;

            return result;
        }
    }

    public IdentityLockResult LockIdentity(LockPayload payload)
    {
        var result = new IdentityLockResult();

        try
        {
            IdentityDatabaseResult getResult = _databaseService.GetIdentity(payload.Id);

            UserIdentity identity = Helper.CheckDatabaseResult<UserIdentity>(getResult, new Exception("Issue getting Identity to lock.")) as UserIdentity;

            LockUserIdentity(identity);

            IdentityDatabaseResult updateResult = _databaseService.UpdateIdentity(identity);

            Helper.CheckDatabaseResult<UserIdentity>(updateResult, new Exception("Issue updating locked Identity."));

            result.LockedIdentity = identity;
            result.GeneratedKey = identity.AccountData.LockKey;
            result.Successful = true;

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.ThrownException = e;
            result.LockedIdentity = null;
            result.GeneratedKey = String.Empty;

            return result;
        }
    }

    public IdentityUpdateResult InvalidateSecurityContext(SecurityContextInvalidationPayload payload)
    {
        var result = new IdentityUpdateResult();

        try
        {
            IdentityDatabaseResult getResult = _databaseService.GetIdentity(payload.Id);
            UserIdentity identity = Helper.CheckDatabaseResult<UserIdentity>(getResult, new Exception("Issue getting Identity to Invalidate."));

            identity.IntegrityStatus = SecurityContextIntegrityStatus.INVALID;

            IdentityDatabaseResult updateResult = _databaseService.UpdateIdentity(identity);
            Helper.CheckDatabaseResult<UserIdentity>(updateResult, new Exception("Issue updating invalidated Identity."));

            result.Successful = true;
            result.UpdatedIdentity = identity;

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.ThrownException = e;
            result.UpdatedIdentity = null;

            return result;
        }
    }

    public IdentityUnlockResult UnlockIdentity(UnlockPayload payload)
    {
        var result = new IdentityUnlockResult();

        try
        {
            IdentityDatabaseResult getResult = _databaseService.GetIdentity(payload.Id);
            UserIdentity identity = Helper.CheckDatabaseResult<UserIdentity>(getResult, new Exception("Issue getting Identity to unlock."));

            if (identity.AccountData.LockKey != payload.Key)
                throw new Exception("Provided Key not valid.");

            UnlockUserIdentity(identity);

            IdentityDatabaseResult updateResult = _databaseService.UpdateIdentity(identity);
            Helper.CheckDatabaseResult<UserIdentity>(updateResult, new Exception("Issue updating unlocked Identity."));

            result.UnlockedIdentity = identity;
            result.Successful = true;

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.ThrownException = e;
            result.UnlockedIdentity = null;

            return result;
        }
    }

    public List<UserIdentity> GetUsers()
    {
        IdentitiesDatabaseResult getResult = _databaseService.GetIdentities();

        return Helper.CheckDatabaseResult<List<UserIdentity>>(getResult, new Exception("Unknown issue getting all Users"));
    }

    public UserIdentity GetUser(ulong id)
    {
        IdentityDatabaseResult getResult = _databaseService.GetIdentity(id);

        return Helper.CheckDatabaseResult<UserIdentity>(getResult, new Exception($"Unknown issue getting User with ID: {id}"));
    }

    /// <summary>
    /// Checks the HashingOptions of an Identity against the current HashingOptions defined in the current options.
    /// If there is a mismatch, the password from the payload gets rehashed with current options and the Identity gets updated
    /// </summary>
    /// <param name="identity"></param>
    /// <param name="payload"></param>
    private void UpdateAuthenticatedUser(UserIdentity identity, AuthenticationPayload payload)
    {
        if (!identity.HashingOptions.IsEqual(_options.PasswordHashOptions))
        {
            PasswordHashResult result = _cryptoService.HashPassword(payload.Password);

            identity.Hash = result.Hash;
            identity.Salt = result.Salt;
            identity.HashingOptions = result.HashingOptions;

            IdentityDatabaseResult updateResult = _databaseService.UpdateIdentity(identity);
            Helper.CheckDatabaseResult<UserIdentity>(updateResult, new Exception("Error Updating User with newly Hashed Password."));
        }
        else
        {
            IdentityDatabaseResult updateResult = _databaseService.UpdateIdentity(identity);
            Helper.CheckDatabaseResult<UserIdentity>(updateResult, new Exception("Error Updating User with Login-Timestamp."));
        }
    }

    private void LockUserIdentity(UserIdentity identity)
    {
        identity.AccountData.Locked = true;
        identity.AccountData.LockKey = _cryptoService.GenerateSemiRandomKey(_options.LockKeyLength);
    }

    private void UnlockUserIdentity(UserIdentity identity)
    {
        identity.AccountData.Locked = false;
        identity.AccountData.LockKey = String.Empty;
    }

}
