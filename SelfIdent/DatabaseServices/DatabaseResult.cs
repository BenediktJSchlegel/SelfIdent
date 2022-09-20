using System;
using System.Collections.Generic;
using SelfIdent.Identity;

namespace SelfIdent.DatabaseServices;

internal class DatabaseResult
{
    internal enum DatabaseResultStatus
    {
        Ok = 0,
        CodeError = 1,
        DbError = 2
    }

    /// <summary>
    /// Status of the Database Operation
    /// </summary>
    public DatabaseResultStatus Status { get; set; }

    /// <summary>
    /// If Database operation caused an error
    /// </summary>
    public bool Successful => Status == DatabaseResultStatus.Ok;

    /// <summary>
    /// Exception if one was thrown
    /// </summary>
    public Exception? ThrownException { get; set; }
}

internal class IdentitiesDatabaseResult : DatabaseResult
{
    /// <summary>
    /// Affected Identities
    /// </summary>
    public List<UserIdentity>? Identities { get; set; }
}

internal class IdentityDatabaseResult : DatabaseResult
{
    /// <summary>
    /// The affected Identity
    /// </summary>
    public UserIdentity? Identity { get; set; }
}

internal class RoleDatabaseResult : DatabaseResult
{
    /// <summary>
    /// The Affected Role
    /// </summary>
    public Roles.Role? Role { get; set; }
}

internal class RolesDatabaseResult : DatabaseResult
{
    /// <summary>
    /// The Affected Roles
    /// </summary>
    public List<Roles.Role>? Roles { get; set; }
}