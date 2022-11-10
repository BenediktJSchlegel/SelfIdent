using System;
using SelfIdent.Account.Authentication;
using SelfIdent.Account.Deletion;
using SelfIdent.Account.Registration;
using SelfIdent.Account.Update;
using SelfIdent.DatabaseServices;
using SelfIdent.Enums;
using SelfIdent.Exceptions;
using SelfIdent.Identity;
using SelfIdent.Interfaces;
using SelfIdent.Options;
using SelfIdent.Cryptography;
using SelfIdent.Validation;
using SelfIdent.Identity.Results;
using SelfIdent.Logging;
using SelfIdent.Account.Lock;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization.Policy;
using SelfIdent.Options.Hashing;
using SelfIdent.Cryptography.Hashing;
using SelfIdent.Helpers;
using SelfIdent.Account.SecurityContext;
using Microsoft.AspNetCore.Http;
using SelfIdent.SecurityContext;
using SelfIdent.Account.SecurityContextInvalidation;
using SelfIdent.Cache;
using SelfIdent.Token;

namespace SelfIdent;

/// <summary>
/// The Main Interface to access NetIdent functionality
/// -
/// Endpoints get exposed through this class but logic is implemented in the IEndpointHandler.
/// -
/// The only logic in this class is for setup purposes.
/// </summary>
public sealed class SelfIdent : ISelfIdentEndpoints
{
    private IDatabaseService _databaseService;
    private SelfIdentOptions _options;
    private SelfIdentMemoryCache _cache;

    internal SelfIdentOptions Options => _options;
    internal SelfIdentMemoryCache MemoryCache => _cache;

    public SelfIdent(SelfIdentOptions options)
    {
        OptionValidator.ValidateSelfIdentOptions(options);

        var setupInfo = new SetupResult() { Successful = true };

        _options = options;
        _cache = new SelfIdentMemoryCache(options.CacheOptions);

        if (_databaseService == null)
            _databaseService = GetDatabaseService();

        if (!_databaseService.CheckConnection())
        {
            setupInfo.ConnectionFailed = true;
            throw new DatabaseConnectionFailedException(options.ConnectionString);
        }

        //Assume Tables exist
        setupInfo.CreatedTables = false;
        setupInfo.FoundExistingTables = true;
        setupInfo.ConnectionFailed = false;

        //Check if Tables exist
        if (!_databaseService.TablesExist())
        {
            //If not => Create them
            _databaseService.CreateTables();
            setupInfo.CreatedTables = true;
            setupInfo.FoundExistingTables = false;
        }

        SetupRoles();
    }

    internal IDatabaseService GetDatabaseService()
    {
        return Options.DatabaseType switch
        {
            DatabaseTypes.MySql => new MySqlService(Options.ConnectionString, Options.DatabaseName, Options),
            DatabaseTypes.MsSql => new MsSqlService(Options.ConnectionString, Options.DatabaseName, Options),
            DatabaseTypes.Oracle => new OracleService(Options.ConnectionString, Options.DatabaseName, Options),
            _ => throw new NotImplementedException()
        };
    }

    private void SetupRoles()
    {
        RolesDatabaseResult getResult = _databaseService.GetAllRoles();

        List<Roles.Role> existingRoles = Helper.CheckDatabaseResult<List<Roles.Role>>(getResult, new Exception("Error getting Roles"));

        if (existingRoles.Count == 0)
        {
            InsertAllRoles(Options.RoleOptions.Roles);
        }
        else
        {
            if (!Roles.Role.RolesMatch(existingRoles, Options.RoleOptions.Roles))
            {
                RolesDatabaseResult deleteResult = _databaseService.DeleteAllRoles();

                Helper.CheckDatabaseResult<List<Roles.Role>>(deleteResult, new Exception("Error deleting Roles"));

                InsertAllRoles(Options.RoleOptions.Roles);

                RolesDatabaseResult cleanupResult = _databaseService.DeleteObsoleteRoleAssignments();

                Helper.CheckDatabaseResult<List<Roles.Role>>(cleanupResult, new Exception("Error deleting obsolete Role Assignments"));
            }
        }
    }

    private void InsertAllRoles(List<Roles.Role> roles)
    {
        foreach (Roles.Role role in roles)
        {
            RoleDatabaseResult result = _databaseService.InsertRole(role);

            Helper.CheckDatabaseResult<Roles.Role>(result, new Exception("Error inserting Roles"));
        }
    }

    private IPasswordHasher GetPasswordHasher(IHashingOptions options)
    {
        switch (options)
        {
            case ArgonHashingOptions argon:
                return new ArgonPasswordHasher(argon);
            case PBKDFHashingOptions pbkdf:
                return new PBKDFPasswordHasher(pbkdf);
            case ScryptHashingOptions scrypt:
                return new ScryptPasswordHasher(scrypt);
            default:
                throw new NotImplementedException();
        }
    }

    public void ConfigureAuthenticationServices(IServiceCollection services, AuthenticationServiceOptions options)
    {
        services.AddAuthentication(options.SchemaName).AddCookie(options.SchemaName, o =>
        {
            o.Cookie.Name = options.SchemaName;
            o.LoginPath = options.LoginPath;
            o.LogoutPath = options.LogoutPath;
            o.EventsType = typeof(AuthenticationEvents.SelfIdentCookieAuthenticationEvents);
        });

        if (options.SecurityContextType == AuthenticationServiceOptions.SecurityContextTypes.Cookie)
            services.AddScoped<AuthenticationEvents.SelfIdentCookieAuthenticationEvents>(s => new AuthenticationEvents.SelfIdentCookieAuthenticationEvents(this));
    }

    public void ConfigureAuthorizationServices(IServiceCollection services, AuthorizationServiceOptions options)
    {
        services.AddAuthorizationCore(o =>
        {
            // Add Policies
        });
    }

    private ISelfIdentEndpoints GenerateEndpointService()
    {
        IDatabaseService databaseService = GetDatabaseService();
        var cryptoService = new CryptographyService(Options.PasswordHashOptions, GetPasswordHasher(Options.PasswordHashOptions));
        var identityService = new IdentityService(databaseService, cryptoService, Options);
        var validator = new Validator(Options.ValidationOptions);
        var contextHandler = new SecurityContextHandler(Options.SecurityContextOptions);

        return new EndpointService(identityService, validator, new Logger(), contextHandler);
    }

    public RegistrationResult Register(RegistrationPayload payload)
    {
        return GenerateEndpointService().Register(payload);
    }

    public UpdateResult Update(UpdatePayload payload)
    {
        return GenerateEndpointService().Update(payload);
    }

    public AuthenticationResult Authenticate(AuthenticationPayload payload)
    {
        return GenerateEndpointService().Authenticate(payload);
    }

    public DeletionResult Delete(DeletionPayload payload)
    {
        return GenerateEndpointService().Delete(payload);
    }

    public LockResult LockAccount(LockPayload payload)
    {
        return GenerateEndpointService().LockAccount(payload);
    }

    public UnlockResult UnlockAccount(UnlockPayload payload)
    {
        return GenerateEndpointService().UnlockAccount(payload);
    }

    public LockResult PermanentelyLockAccount(LockPayload payload)
    {
        return GenerateEndpointService().PermanentelyLockAccount(payload);
    }

    public object GeneratePasswordResetKey(object payload)
    {
        return GenerateEndpointService().GeneratePasswordResetKey(payload);
    }

    public List<User> GetAllUsers()
    {
        return GenerateEndpointService().GetAllUsers();
    }

    public User GetUser(ulong id)
    {
        return GenerateEndpointService().GetUser(id);
    }

    public AuthenticationResult CookieAuthenticate(CookieAuthenticationPayload payload)
    {
        return GenerateEndpointService().CookieAuthenticate(payload);
    }

    public void CookieSignOut(HttpContext context)
    {
        GenerateEndpointService().CookieSignOut(context);
    }

    public TokenAuthenticationResult TokenAuthenticate(AuthenticationPayload payload)
    {
        return GenerateEndpointService().TokenAuthenticate(payload);
    }

    public SecurityContextInvalidationResult InvalidateSecurityContext(SecurityContextInvalidationPayload payload)
    {
        return GenerateEndpointService().InvalidateSecurityContext(payload);
    }

    public TokenUserValidationResult ValidateToken(TokenValidationPayload payload)
    {
        return GenerateEndpointService().ValidateToken(payload);
    }
}
