using System;
using SelfIdent.Enums;
using SelfIdent.Exceptions;
using SelfIdent.Helpers;
using SelfIdent.Interfaces;
using SelfIdent.Options.Hashing;
using System.Collections.Generic;
using SelfIdent.Constants;

namespace SelfIdent.Identity;

/// <summary>
/// Class representing a single user within this library.
/// -
/// A UserIdentity gets created when a user is registered.
/// A UserIdentity gets generated from DB-Data when there is a check against the database.
/// -
/// UserIdentities get passed to the IDatabaseServices to insert, update and delete users.
/// </summary>
internal class UserIdentity
{
    /// <summary>
    /// Unique ID
    /// </summary>
    public ulong Id { get; set; }
    /// <summary>
    /// User Email
    /// </summary>
    public string? Email { get; set; }
    /// <summary>
    /// Username
    /// </summary>
    public string? Name { get; set; }
    /// <summary>
    /// Hashed Password in Base64
    /// </summary>
    public string Hash { get; set; }
    /// <summary>
    /// Salt in Base64
    /// </summary>
    public string Salt { get; set; }
    /// <summary>
    /// Roles Assigned to the User
    /// </summary>
    public List<Roles.Role> Roles { get; set; }
    /// <summary>
    /// Options that the Password is hashed with
    /// </summary>
    public IHashingOptions HashingOptions { get; set; }
    /// <summary>
    /// The Hashfunction to be used
    /// </summary>
    public HashFunctionTypes HashFunctionType { get; set; }
    /// <summary>
    /// Data describing the state of the user account
    /// </summary>
    public IdentityAccountStatus AccountData { get; set; }
    /// <summary>
    /// Data describing the current Authentication-Status of the user
    /// </summary>
    public IdentityAuthenticationStatus AuthenticationData { get; set; }
    /// <summary>
    /// If a SecurityContext based on this Identity is still valid, must be reauthorized or updated
    /// </summary>
    public SecurityContextIntegrityStatus IntegrityStatus { get; set; } = SecurityContextIntegrityStatus.VALID;

    public UserIdentity(string? username, string? email, string hash, string salt)
    {
        this.AccountData = new IdentityAccountStatus();
        this.AuthenticationData = new IdentityAuthenticationStatus();
        this.HashingOptions = new PBKDFHashingOptions();

        this.Roles = new List<Roles.Role>();

        this.Name = username;
        this.Email = email;
        this.Hash = hash;
        this.Salt = salt;
    }

    public UserIdentity(System.Data.DataRow row)
    {
        this.AccountData = new IdentityAccountStatus();
        this.AuthenticationData = new IdentityAuthenticationStatus();

        this.Id = Helper.SafelySet<ulong>(row[NameConstants.COL_ID]);
        this.Email = Helper.SafelySet<string>(row[NameConstants.COL_EMAIL]);
        this.Name = Helper.SafelySet<string>(row[NameConstants.COL_NAME]);
        this.IntegrityStatus = (SecurityContextIntegrityStatus)Helper.SafelySet<int>(row[NameConstants.COL_INTEGRITYSTATUS]);

        string? hash = Helper.SafelySet<string>(row[NameConstants.COL_HASH]);
        string? salt = Helper.SafelySet<string>(row[NameConstants.COL_SALT]);

        if (String.IsNullOrEmpty(hash) || String.IsNullOrEmpty(salt))
        {
            throw new CriticalStateException("Found Identity in Database with no Salt or Password");
        }
        else
        {
            this.Hash = hash;
            this.Salt = salt;
        }

        this.AccountData.Locked = Helper.SafelySet<bool>(row[NameConstants.COL_LOCKED]);
        this.AccountData.LockKey = Helper.SafelySet<string>(row[NameConstants.COL_LOCKKEY]);
        this.AccountData.LoginAttempts = Helper.SafelySet<int>(row[NameConstants.COL_ATTEMPTS]);
        this.AccountData.LastLogin = Helper.SafelySet<DateTime>(row[NameConstants.COL_LASTLOGON]);
        this.AccountData.RegistrationTime = Helper.SafelySet<DateTime>(row[NameConstants.COL_REGISTERDATE]);

        this.Roles = new List<Roles.Role>();

        this.HashFunctionType = (HashFunctionTypes)Helper.SafelySet<int>(row[NameConstants.COL_HASHINGFUNCTION]);
        this.HashingOptions = SetHashingOptionsByDataRow(row);
        this.HashingOptions.SaltByteLength = Helper.SafelySet<int>(row[NameConstants.COL_SALTBYTELENGTH]);
        this.HashingOptions.HashByteLength = Helper.SafelySet<int>(row[NameConstants.COL_PASSWORDBYTELENGTH]);
        this.HashingOptions.Iterations = Helper.SafelySet<int>(row[NameConstants.COL_ITERATIONS]);
    }

    public UserIdentity Clone()
    {
        var clone = new UserIdentity(this.Name, this.Email, this.Hash, this.Salt);

        clone.HashFunctionType = this.HashFunctionType;
        clone.Id = this.Id;
        clone.IntegrityStatus = this.IntegrityStatus;

        clone.Roles = new List<Roles.Role>();

        foreach (Roles.Role role in this.Roles)
        {
            clone.Roles.Add(role.Clone());
        }

        clone.AuthenticationData = new IdentityAuthenticationStatus();
        clone.AccountData = new IdentityAccountStatus();
        clone.HashingOptions = CloneHashingOptions();

        clone.AccountData.LastLogin = this.AccountData.LastLogin;
        clone.AuthenticationData.Authenticated = this.AuthenticationData.Authenticated;
        clone.AccountData.Locked = this.AccountData.Locked;
        clone.AccountData.LockKey = this.AccountData.LockKey;
        clone.AccountData.LoginAttempts = this.AccountData.LoginAttempts;
        clone.AccountData.RegistrationTime = this.AccountData.RegistrationTime;

        return clone;
    }

    private IHashingOptions CloneHashingOptions()
    {
        IHashingOptions? result;

        switch (this.HashingOptions)
        {
            case ArgonHashingOptions argon:
                result = new ArgonHashingOptions();

                ((ArgonHashingOptions)result).TimeInSeconds = argon.TimeInSeconds;
                ((ArgonHashingOptions)result).MemoryInKB = argon.MemoryInKB;
                ((ArgonHashingOptions)result).Threads = argon.Threads;
                break;
            case PBKDFHashingOptions pbkdf:
                result = new PBKDFHashingOptions();

                ((PBKDFHashingOptions)result).HashingFunction = pbkdf.HashingFunction;
                break;
            case ScryptHashingOptions scrypt:
                result = new ScryptHashingOptions();

                ((ScryptHashingOptions)result).BlockSize = scrypt.BlockSize;
                ((ScryptHashingOptions)result).Threads = scrypt.Threads;
                break;
            default:
                throw new NotImplementedException();
        }

        result.SaltByteLength = this.HashingOptions.SaltByteLength;
        result.HashByteLength = this.HashingOptions.HashByteLength;
        result.Iterations = this.HashingOptions.Iterations;

        return result;
    }

    private IHashingOptions SetHashingOptionsByDataRow(System.Data.DataRow row)
    {
        switch (this.HashFunctionType)
        {
            case HashFunctionTypes.Argon:
                var argonOptions = new ArgonHashingOptions();

                argonOptions.MemoryInKB = Helper.SafelySet<int>(row[NameConstants.COL_ARGONMEMORY]);
                argonOptions.Threads = Helper.SafelySet<int>(row[NameConstants.COL_ARGONTHREADS]);
                argonOptions.TimeInSeconds = Helper.SafelySet<int>(row[NameConstants.COL_ARGONTIME]);

                return argonOptions;
            case HashFunctionTypes.PBKDF:
                var pbkdfOptions = new PBKDFHashingOptions();
                pbkdfOptions.HashingFunction = (Microsoft.AspNetCore.Cryptography.KeyDerivation.KeyDerivationPrf)Helper.SafelySet<int>(row[NameConstants.COL_PBKDFFUNCTION]);

                return pbkdfOptions;
            case HashFunctionTypes.Scrypt:
                var scryptOptions = new ScryptHashingOptions();

                scryptOptions.BlockSize = Helper.SafelySet<int>(row[NameConstants.COL_SCRYPTBLOCKSIZE]);
                scryptOptions.Threads = Helper.SafelySet<int>(row[NameConstants.COL_SCRYPTTHREADS]);

                return scryptOptions;
            default:
                throw new CriticalStateException("Found Identity with no HashFunctionType.");
        }
    }

    /// <summary>
    /// Generates a Public-User Object from this UserIdentity.
    /// </summary>
    /// <returns></returns>
    public User ToPublicUser()
    {
        return new User(this);
    }
}