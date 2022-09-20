using System;
using SelfIdent.Interfaces;
using SelfIdent.Identity;
using SelfIdent.Options;
using System.Collections.Generic;
using SelfIdent.Roles;

namespace SelfIdent.DatabaseServices;

internal class MsSqlService : IDatabaseService
{
    public string ConnectionString { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public string DatabaseName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    public MsSqlService(string connectionString, string dbName, SelfIdentOptions options)
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

    public DatabaseResult Delete(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    public DatabaseResult Get(ulong id)
    {
        throw new NotImplementedException();
    }

    public DatabaseResult Get(string username, string email)
    {
        throw new NotImplementedException();
    }

    public bool IdentityExists(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    public DatabaseResult Insert(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    public void Setup(string connectionString, string dbName, SelfIdentOptions options)
    {
        throw new NotImplementedException();
    }

    public DatabaseResult Update(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    public IdentitiesDatabaseResult GetIdentities()
    {
        throw new NotImplementedException();
    }

    IdentityDatabaseResult IDatabaseService.InsertIdentity(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    IdentityDatabaseResult IDatabaseService.DeleteIdentity(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    IdentityDatabaseResult IDatabaseService.UpdateIdentity(UserIdentity identity)
    {
        throw new NotImplementedException();
    }

    IdentityDatabaseResult IDatabaseService.GetIdentity(ulong id)
    {
        throw new NotImplementedException();
    }

    public RoleDatabaseResult DeleteObsoleteRoleAssignments()
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

    RolesDatabaseResult IDatabaseService.DeleteObsoleteRoleAssignments()
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