using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using SelfIdent.Account.Authentication;
using SelfIdent.Account.Deletion;
using SelfIdent.Account.Lock;
using SelfIdent.Account.Registration;
using SelfIdent.Account.SecurityContext;
using SelfIdent.Account.SecurityContextInvalidation;
using SelfIdent.Account.Update;
using SelfIdent.Exceptions;
using SelfIdent.Identity.Results;
using SelfIdent.Interfaces;
using SelfIdent.Validation;

namespace SelfIdent;

internal class EndpointService : ISelfIdentEndpoints
{
    private ISecurityContextHandler _securityContextHandler;
    private IIdentityService _identityService;
    private IValidator _validator;
    private ILogger _logger;

    public EndpointService(IIdentityService identityService, IValidator validator, ILogger logger, ISecurityContextHandler contextHandler)
    {
        this._identityService = identityService;
        this._validator = validator;
        this._logger = logger;
        this._securityContextHandler = contextHandler;
    }

    public RegistrationResult Register(RegistrationPayload payload)
    {
        var result = new RegistrationResult(payload);

        if (payload == null)
            throw new ArgumentNullException(nameof(payload));

        ValidationResult validationResult = _validator.Validate(payload);

        if (validationResult.IsInvalid)
        {
            result.Successful = false;
            result.ThrownException = new ValidationFailedException(payload);

            return result;
        }

        IdentityCreationResult creationResult = _identityService.CreateIdentity(payload);

        if (!creationResult.Successful || creationResult.CreatedIdentity == null)
        {
            result.Successful = false;
            result.ThrownException = creationResult.ThrownException;

            if (result.ThrownException == null)
                result.ThrownException = new Exception("Error creating Identity.");

            return result;
        }
        else
        {
            result.GeneratedUser = creationResult.CreatedIdentity.ToPublicUser();

            return result;
        }
    }

    public AuthenticationResult Authenticate(AuthenticationPayload payload)
    {
        var result = new AuthenticationResult();

        try
        {
            IdentityAuthenticationResult authenticationResult = _identityService.AuthenticateIdentity(payload);

            if (!authenticationResult.Successful)
            {
                result.Successful = false;
                result.Status = AuthenticationResultStatus.FAILED_UNKNOWN;
                result.ThrownException = authenticationResult.ThrownException;
                result.User = null;

                return result;
            }

            if (!authenticationResult.UserExists || authenticationResult.AuthenticatedIdentity == null)
            {
                result.Successful = false;
                result.Status = AuthenticationResultStatus.FAILED_UNKNOWNUSER;
                result.ThrownException = null;
                result.User = null;

                return result;
            }

            if (!authenticationResult.AuthenticatedIdentity.AuthenticationData.Authenticated)
            {
                result.Successful = false;
                result.Status = AuthenticationResultStatus.FAILED_BADINPUT;
                result.ThrownException = null;
                result.User = null;

                return result;
            }

            if (authenticationResult.AuthenticatedIdentity.AccountData.Locked)
            {
                result.Successful = false;
                result.Status = AuthenticationResultStatus.FAILED_LOCKED;
                result.ThrownException = null;
                result.User = authenticationResult.AuthenticatedIdentity.ToPublicUser();

                return result;
            }

            result.Successful = true;
            result.Status = AuthenticationResultStatus.OK;
            result.User = authenticationResult.AuthenticatedIdentity.ToPublicUser();

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.Status = AuthenticationResultStatus.FAILED_UNKNOWN;
            result.ThrownException = e;
            result.User = null;

            return result;
        }
    }

    public DeletionResult Delete(DeletionPayload payload)
    {
        var result = new DeletionResult();
        result.Successful = true;

        try
        {
            IdentityDeletionResult deletionResult = _identityService.DeleteIdentity(payload);

            if (!deletionResult.Successful || deletionResult.DeletedIdentity == null)
            {
                if (result.ThrownException != null)
                    throw result.ThrownException;

                throw new Exception($"Failed deleting ID: {payload.Id}");
            }

            result.User = deletionResult.DeletedIdentity.ToPublicUser();
            result.Successful = true;

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.ThrownException = e;
            result.User = null;

            return result;
        }
    }

    public LockResult LockAccount(LockPayload payload)
    {
        var result = new LockResult(payload);

        try
        {
            IdentityLockResult lockResult = _identityService.LockIdentity(payload);

            if (!lockResult.Successful || lockResult.LockedIdentity == null)
            {
                if (result.ThrownException != null)
                    throw result.ThrownException;

                throw new Exception($"Failed locking Account with ID: {payload.Id}");
            }

            result.Successful = true;
            result.LockKey = lockResult.GeneratedKey;
            result.User = lockResult.LockedIdentity.ToPublicUser();

            return result;
        }
        catch (Exception e)
        {
            result.ThrownException = e;
            result.Successful = false;
            result.User = null;

            return result;
        }
    }

    public UnlockResult UnlockAccount(UnlockPayload payload)
    {
        var result = new UnlockResult(payload);

        try
        {
            IdentityUnlockResult lockResult = _identityService.UnlockIdentity(payload);

            if (!lockResult.Successful || lockResult.UnlockedIdentity == null)
            {
                if (result.ThrownException != null)
                    throw result.ThrownException;

                throw new Exception($"Failed unlocking Account with ID: {payload.Id}");
            }

            result.Successful = true;
            result.User = lockResult.UnlockedIdentity.ToPublicUser();

            return result;
        }
        catch (Exception e)
        {
            result.ThrownException = e;
            result.Successful = false;
            result.User = null;

            return result;
        }
    }

    public List<User> GetAllUsers()
    {
        var users = new List<User>();

        List<Identity.UserIdentity> allIdentities = _identityService.GetUsers();

        foreach (Identity.UserIdentity identity in allIdentities)
        {
            users.Add(identity.ToPublicUser());
        }

        return users;
    }

    public User GetUser(ulong id)
    {
        return _identityService.GetUser(id).ToPublicUser();
    }

    public UpdateResult Update(UpdatePayload payload)
    {
        var result = new UpdateResult(payload);

        try
        {
            IdentityUpdateResult updateResult = _identityService.UpdateIdentity(payload);

            if (!updateResult.Successful || updateResult.UpdatedIdentity == null)
            {
                if (result.ThrownException != null)
                    throw result.ThrownException;

                throw new Exception($"Failed deleting ID: {payload.Id}");
            }

            result.Successful = true;
            result.User = updateResult.UpdatedIdentity.ToPublicUser();

            return result;
        }
        catch (Exception e)
        {
            result.ThrownException = e;
            result.Successful = false;
            result.User = null;

            return result;
        }
    }

    public LockResult PermanentelyLockAccount(LockPayload payload)
    {
        throw new NotImplementedException();
    }

    public object GeneratePasswordResetKey(object payload)
    {
        throw new NotImplementedException();
    }

    public AuthenticationResult CookieAuthenticate(CookieAuthenticationPayload payload)
    {
        var result = new CookieAuthenticationResult();

        try
        {
            AuthenticationResult authenticationResult = Authenticate(payload);

            if (authenticationResult.Status != AuthenticationResultStatus.OK || authenticationResult.User == null)
            {
                return authenticationResult;
            }
            else
            {
                var contextPayload = new CookieContextPayload(payload.Context, authenticationResult.User);

                contextPayload.User = authenticationResult.User;
                contextPayload.PersistentCookie = payload.PersistentCookie;
                contextPayload.ExpiresUtc = payload.ExpiresUtc;
                contextPayload.AllowRefresh = payload.AllowRefresh;
                contextPayload.RedirectUri = payload.RedirectUri;

                _securityContextHandler.AuthenticateCookie(contextPayload);

                result.Status = AuthenticationResultStatus.OK;

                return result;
            }
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.Status = AuthenticationResultStatus.FAILED_UNKNOWN;
            result.ThrownException = e;
            result.User = null;

            return result;
        }
    }

    public void CookieSignOut(HttpContext context)
    {
        _securityContextHandler.LogoutCookie(context);
    }

    public TokenAuthenticationResult TokenAuthenticate(AuthenticationPayload payload)
    {
        var result = new TokenAuthenticationResult();

        try
        {
            AuthenticationResult authenticationResult = Authenticate(payload);

            if (authenticationResult.Status != AuthenticationResultStatus.OK || authenticationResult.User == null)
            {
                return new TokenAuthenticationResult(authenticationResult);
            }
            else
            {
                var contextPayload = new TokenContextPayload(payload.Context, authenticationResult.User);

                contextPayload.User = authenticationResult.User;

                TokenContextResult tokenResult = _securityContextHandler.AuthenticateToken(contextPayload);

                if (!tokenResult.Successful)
                {
                    if (tokenResult.ThrownException != null)
                        throw tokenResult.ThrownException;

                    throw new Exception("Error generating AuthenticationToken");
                }

                result.Token = tokenResult.Token;
                result.Status = AuthenticationResultStatus.OK;
                result.User = authenticationResult.User;
                result.Successful = true;

                return result;
            }
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.Status = AuthenticationResultStatus.FAILED_UNKNOWN;
            result.ThrownException = e;
            result.User = null;
            result.Token = null;

            return result;
        }
    }

    public SecurityContextInvalidationResult InvalidateSecurityContext(SecurityContextInvalidationPayload payload)
    {
        var result = new SecurityContextInvalidationResult(payload);

        try
        {
            IdentityUpdateResult updateResult = _identityService.InvalidateSecurityContext(payload);

            if (!updateResult.Successful || updateResult.UpdatedIdentity == null)
            {
                if (result.ThrownException != null)
                    throw result.ThrownException;

                throw new Exception($"Failed Invalidating ID: {payload.Id}");
            }

            result.Successful = true;
            result.InvalidatedUser = updateResult.UpdatedIdentity.ToPublicUser();

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.ThrownException = e;
            result.InvalidatedUser = null;

            return result;
        }
    }

}
