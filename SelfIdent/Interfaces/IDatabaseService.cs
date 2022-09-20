using System.Collections.Generic;
using SelfIdent.DatabaseServices;
using SelfIdent.Identity;
using SelfIdent.Options;

namespace SelfIdent.Interfaces;

internal interface IDatabaseService
{
    /// <summary>
    /// Inserts a new Identity
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    IdentityDatabaseResult InsertIdentity(UserIdentity identity);
    /// <summary>
    /// Deletes this Identity
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    IdentityDatabaseResult DeleteIdentity(UserIdentity identity);
    /// <summary>
    /// Updates the Database-Values for this Identity.
    /// Exluding Roles
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    IdentityDatabaseResult UpdateIdentity(UserIdentity identity);
    /// <summary>
    /// Updates the Roles for this Identity
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    IdentityDatabaseResult InsertIdentityRole(ulong userId, Roles.Role role);
    /// <summary>
    /// Deletes Role-Assignments for this Identity
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    IdentityDatabaseResult DeleteIdentityRoles(UserIdentity identity);
    /// <summary>
    /// Gets a fully filled Identity by ID
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    IdentityDatabaseResult GetIdentity(ulong id);
    /// <summary>
    /// Gets a fully filled Identity by username, email or both
    /// </summary>
    /// <param name="username"></param>
    /// <param name="email"></param>
    /// <returns></returns>
    IdentityDatabaseResult GetIdentity(string? username, string? email);
    /// <summary>
    /// Gets all Identities
    /// </summary>
    /// <returns></returns>
    IdentitiesDatabaseResult GetIdentities();
    /// <summary>
    /// Checks if the given Identity exists in the Database
    /// </summary>
    /// <param name="identity"></param>
    /// <returns></returns>
    bool IdentityExists(UserIdentity identity);
    /// <summary>
    /// Checks if Tables defined in the TableDefinitions are present
    /// </summary>
    /// <returns></returns>
    bool TablesExist();
    /// <summary>
    /// Creates missing Tables from the DatabaseDefinitions
    /// </summary>
    void CreateTables();
    /// <summary>
    /// Checks if a connection can be established
    /// </summary>
    /// <returns></returns>
    bool CheckConnection();
    /// <summary>
    /// Inserts a Role to the database
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    RoleDatabaseResult InsertRole(Roles.Role role);
    /// <summary>
    /// Gets all Roles
    /// </summary>
    /// <returns></returns>
    RolesDatabaseResult GetAllRoles();
    /// <summary>
    /// Deletes all Roles
    /// </summary>
    /// <returns></returns>
    RolesDatabaseResult DeleteAllRoles();
    /// <summary>
    /// Deletes all Role-Asssignments that point to a Role that has been removed
    /// </summary>
    /// <returns></returns>
    RolesDatabaseResult DeleteObsoleteRoleAssignments();
    /// <summary>
    /// Runs Database Setups
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="dbName"></param>
    /// <param name="options"></param>

    string ConnectionString { get; set; }
    string DatabaseName { get; set; }
}