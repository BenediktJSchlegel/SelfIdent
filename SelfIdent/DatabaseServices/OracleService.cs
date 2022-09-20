using System;
using System.Collections.Generic;
using SelfIdent.Identity;
using SelfIdent.Interfaces;
using SelfIdent.Options;
using SelfIdent.Roles;

namespace SelfIdent.DatabaseServices;

internal class OracleService : IDatabaseService
{
    public string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string DatabaseName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public OracleService(string connectionString, string dbName, SelfIdentOptions options)
    {
        throw new NotImplementedException();
    }

    public bool CheckConnection()
    {
        throw new NotImplementedException();
    }

    public bool TablesExist()
    {
        throw new NotImplementedException();
    }

    public void CreateTables()
    {
        throw new NotImplementedException();
    }

    public IdentityDatabaseResult InsertIdentity(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    public IdentityDatabaseResult DeleteIdentity(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    public IdentityDatabaseResult UpdateIdentity(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    public IdentityDatabaseResult GetIdentity(ulong id)
    {
        throw new NotImplementedException();
    }

    public IdentitiesDatabaseResult GetIdentities()
    {
        throw new NotImplementedException();
    }

    public bool IdentityExists(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    public RoleDatabaseResult InsertRole(Role roles)
    {
        throw new NotImplementedException();
    }

    public RolesDatabaseResult GetAllRoles()
    {
        throw new NotImplementedException();
    }

    public RolesDatabaseResult DeleteAllRoles()
    {
        throw new NotImplementedException();
    }

    public RolesDatabaseResult DeleteObsoleteRoleAssignments()
    {
        throw new NotImplementedException();
    }

    public void Setup(string connectionString, string dbName, SelfIdentOptions options)
    {
        throw new NotImplementedException();
    }

    public IdentityDatabaseResult InsertIdentityRole(ulong userId, Role role)
    {
        throw new NotImplementedException();
    }

    public IdentityDatabaseResult DeleteIdentityRoles(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    public IdentityDatabaseResult GetIdentity(string? username, string? email)
    {
        throw new NotImplementedException();
    }
}