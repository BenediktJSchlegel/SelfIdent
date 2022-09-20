using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Exceptions;
using SelfIdent.Interfaces;
using SelfIdent.Options;
using SelfIdent.Options.Hashing;

namespace SelfIdent.Validation;

internal static class OptionValidator
{
    public static void ValidateSelfIdentOptions(SelfIdentOptions options)
    {
        if (options == null)
            throw new InvalidOptionException("No Options given.");

        if (options.MultiFactorAuthenticationActive)
            ValidateMFAOptions(options.MFAOptions);

        if (options.DatabaseType == Enums.DatabaseTypes.Unknown)
            throw new InvalidOptionException("No DatabaseType given.");

        if (String.IsNullOrEmpty(options.DatabaseName))
            throw new InvalidOptionException("No Databasename given");

        if (options.HashFunctionType == Enums.HashFunctionTypes.Unknown)
            throw new InvalidOptionException("No HashFunctionType given.");

        if (options.LockAccountOnRegistration && options.LockKeyLength <= 0)
            throw new InvalidOptionException($"{nameof(options.LockAccountOnRegistration)} is true but {nameof(options.LockKeyLength)} is <= 0");

        if (String.IsNullOrEmpty(options.ConnectionString))
            throw new InvalidOptionException("No ConnectionString given");

        ValidateRoleOptions(options.RoleOptions);
        ValidatePasswordHashOptions(options.PasswordHashOptions);
        ValidateValidationOptions(options.ValidationOptions);
        ValidateSecurityContextOptions(options.SecurityContextOptions);

        if ((options.HashFunctionType == Enums.HashFunctionTypes.Argon && !(options.PasswordHashOptions is ArgonHashingOptions)) ||
            (options.HashFunctionType == Enums.HashFunctionTypes.Scrypt && !(options.PasswordHashOptions is ScryptHashingOptions)) ||
            options.HashFunctionType == Enums.HashFunctionTypes.PBKDF && !(options.PasswordHashOptions is PBKDFHashingOptions))
            throw new InvalidOptionException($"HashFunctionType {options.HashFunctionType} does not Match HashOptions of Type {typeof(Options.PasswordValidationOptions)}");
    }

    private static void ValidateRoleOptions(RoleOptions options)
    {
        if (options == null)
            throw new InvalidOptionException("No RoleOptions given.");

        if (options.Roles == null || options.Roles.Count <= 0)
            throw new InvalidOptionException("No Roles given.");

        if (options.DefaultRoles == null || options.DefaultRoles.Count <= 0)
            throw new InvalidOptionException("No Default-Roles given.");
    }

    private static void ValidateSecurityContextOptions(SecurityContextOptions options)
    {
        if (options == null)
            throw new InvalidOptionException("No SecurityContextOptions given.");

        if (options.Type != SecurityContextOptions.SecurityContextAuthenticationTypes.None)
        {
            if (String.IsNullOrEmpty(options.AuthenticationSchema))
                throw new InvalidOptionException("Invalid AuthenticationSchema");

            if (options.Type != SecurityContextOptions.SecurityContextAuthenticationTypes.Cookies && String.IsNullOrEmpty(options.TokenSecretKey))
                throw new InvalidOptionException("No TokenSecretKey provided but Token-Authentication is active");
        }
    }

    private static void ValidateMFAOptions(MFAOptions options)
    {
        if (options.KeyLength <= 0)
            throw new InvalidOptionException($"{nameof(options.KeyLength)} must be greater than 0.");

        if (options.KeyTimeout.TotalSeconds <= 0)
            throw new InvalidOptionException($"{nameof(options.KeyTimeout)} is too short.");
    }

    private static void ValidateValidationOptions(ValidationOptions options)
    {
        // ValidationOptions being null is fine.
        // Validation for a certain Type if Input being ON and Options for that Input being null is not fine!
        if (options == null)
            return;

        if (options.ValidatePasswordOnRegister)
        {
            if (options.PasswordValidationOptions == null)
                throw new InvalidOptionException($"{nameof(options.ValidatePasswordOnRegister)} is true but {nameof(options.PasswordValidationOptions)} is null.");

        }

        if (options.ValidateEmailOnRegister)
        {
            if (options.EmailValidationOptions == null)
                throw new InvalidOptionException($"{nameof(options.ValidateEmailOnRegister)} is true but {nameof(options.EmailValidationOptions)} is null.");

        }

        if (options.ValidateUsernameOnRegister)
        {
            if (options.UsernameValidationOptions == null)
                throw new InvalidOptionException($"{nameof(options.ValidateUsernameOnRegister)} is true but {nameof(options.UsernameValidationOptions)} is null.");

        }
    }

    private static void ValidatePasswordHashOptions(IHashingOptions options)
    {
        if (options == null)
            throw new InvalidOptionException("No HashOptions were given.");

        if (options.Iterations <= 0 || options.HashByteLength <= 0 || options.SaltByteLength <= 0)
            throw new InvalidOptionException($"{nameof(options.Iterations)}, {nameof(options.HashByteLength)} and {nameof(options.SaltByteLength)} must be greater than 0");

        switch (options)
        {
            case ArgonHashingOptions argon:
                if (argon.MemoryInKB <= 0 || argon.Threads <= 0 || argon.TimeInSeconds <= 0)
                    throw new InvalidOptionException($"{nameof(argon.MemoryInKB)}, {nameof(argon.Threads)} and {nameof(argon.TimeInSeconds)} must be greater than 0");
                break;
            case PBKDFHashingOptions pbkdf:
                break;
            case ScryptHashingOptions scrypt:
                if (scrypt.Threads <= 0 || scrypt.BlockSize <= 0)
                    throw new InvalidOptionException($"{nameof(scrypt.Threads)} and {nameof(scrypt.BlockSize)} must be greater than 0");
                break;
            default:
                break;
        }
    }
}
