using MySql.Data.MySqlClient;
using SelfIdent.DatabaseDefinitions;
using SelfIdent.Exceptions;
using SelfIdent.Identity;
using SelfIdent.Interfaces;
using System;
using System.Linq;
using System.Text;
using SelfIdent.Options;
using System.Data;
using SelfIdent.Options.Hashing;
using System.Collections.Generic;
using SelfIdent.Roles;
using SelfIdent.Constants;

namespace SelfIdent.DatabaseServices;

internal class MySqlService : IDatabaseService
{
    private SelfIdentOptions _options;
    private MySqlConnection _connection;

    public MySqlService(string connectionString, string dbName, SelfIdentOptions options)
    {
        if (String.IsNullOrEmpty(connectionString))
            throw new ArgumentNullException(nameof(connectionString));

        if (String.IsNullOrEmpty(dbName))
            throw new ArgumentNullException(nameof(dbName));

        if (options == null)
            throw new ArgumentException(nameof(options));

        ConnectionString = connectionString;
        DatabaseName = dbName;

        _connection = new MySqlConnection(ConnectionString);
        _options = options;
    }

    private string _conString = String.Empty;
    public string ConnectionString
    {
        get => _conString;
        set
        {
            _conString = value;
        }
    }

    private string _dbName = String.Empty;
    public string DatabaseName
    {
        get => _dbName;
        set
        {
            _dbName = value;
        }
    }

    /// <summary>
    /// Checks if all Tables that are defined in the TableDefinitions exist
    /// </summary>
    /// <returns></returns>
    public bool TablesExist()
    {
        foreach (TableDefinition table in TableDefinitions.GetTableDefinitions(_options))
        {
            if (!TableExists(table))
                return false;
        }

        return true;
    }

    private bool TableExists(TableDefinition table)
    {
        StringBuilder sql = new StringBuilder();

        sql.Append("SELECT * ");
        sql.Append("FROM INFORMATION_SCHEMA.TABLES ");
        sql.Append("WHERE ");
        sql.Append("`TABLE_SCHEMA` = @TABLE_SCHEMA ");
        sql.Append("AND ");
        sql.Append("`TABLE_NAME` = @TABLE_NAME");

        using (_connection = new MySqlConnection(ConnectionString))
        {
            _connection.Open();

            var command = new MySqlCommand(sql.ToString(), _connection);
            command.Parameters.Add("@TABLE_SCHEMA", MySqlDbType.VarChar).Value = DatabaseName;
            command.Parameters.Add("@TABLE_NAME", MySqlDbType.VarChar).Value = table.Name;

            var reader = command.ExecuteReader();

            if (!reader.HasRows)
                return false;

            return true;
        }
    }

    private MySqlDbType TypeToMySqlDbType(DataTypes type)
    {
        switch (type)
        {
            case DataTypes.String:
                return MySqlDbType.VarChar;
            case DataTypes.Int:
                return MySqlDbType.Int32;
            case DataTypes.Double:
                return MySqlDbType.Double;
            case DataTypes.Float:
                return MySqlDbType.Float;
            case DataTypes.Bool:
                return MySqlDbType.Bit;
            case DataTypes.Long:
                return MySqlDbType.Int64;
            default:
                return MySqlDbType.Blob;
        }
    }

    private string TypeToMySqlTypeString(DataTypes type)
    {
        switch (type)
        {
            case DataTypes.String:
                return "VARCHAR";
            case DataTypes.Int:
                return "INT";
            case DataTypes.Double:
                return "DOUBLE";
            case DataTypes.Float:
                return "FLOAT";
            case DataTypes.Bool:
                return "BIT";
            case DataTypes.Long:
                return "BIGINT";
            case DataTypes.DateTime:
                return "DATETIME";
            default:
                return "BLOB";
        }
    }

    private string TableDefinitionToMySqlScript(TableDefinition table)
    {
        StringBuilder sql = new StringBuilder();
        sql.Append("CREATE TABLE " + DatabaseName + "." + table.Name);
        //Opening Brace
        sql.Append("(");

        //Columns
        foreach (TableColumn column in table.Columns)
        {
            sql.Append(column.Name);
            sql.Append(" ");

            if (column.Length == null)
            {
                //Default Length => No Braces
                sql.Append(TypeToMySqlTypeString(column.Type));
            }
            else
            {
                //Non-Default Length => Braces
                sql.Append(TypeToMySqlTypeString(column.Type));
                sql.Append("(");
                sql.Append(column.Length.ToString());
                sql.Append(")");
            }

            if (column.IsPrimaryKey)
                sql.Append(" AUTO_INCREMENT PRIMARY KEY ");

            if (!column.Nullable && !column.IsPrimaryKey)
                sql.Append(" NOT NULL ");

            if (table.Columns.IndexOf(column) == table.Columns.IndexOf(table.Columns.Last()))
            {
                //Is last Index => no ,
                sql.Append(" ");
            }
            else
            {
                //Not last Index => ,
                sql.Append(",");
                sql.Append(" ");
            }
        }

        //End Brace
        sql.Append(");");

        return sql.ToString();
    }


    public void CreateTables()
    {
        foreach (TableDefinition table in TableDefinitions.GetTableDefinitions(_options))
        {
            if (TableExists(table))
                continue;

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                string sql = TableDefinitionToMySqlScript(table);

                var command = new MySqlCommand(sql, _connection);

                command.ExecuteScalar();
            }
        }
    }

    public bool CheckConnection()
    {
        if (_connection == null)
            throw new SetupNotCompleteException();

        try
        {
            using (_connection)
            {
                _connection.Open();
                _connection.Close();

                return true;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool IdentityExists(UserIdentity identity)
    {
        var sql = new StringBuilder();

        sql.Append($"SELECT * FROM {NameConstants.TABLE_MAINUSER} WHERE");
        sql.Append($"({NameConstants.COL_NAME} = @{NameConstants.COL_NAME}");
        sql.Append(" OR ");
        sql.Append($"{NameConstants.COL_EMAIL} = @{NameConstants.COL_EMAIL});");

        using (_connection = new MySqlConnection(ConnectionString))
        {
            _connection.Open();

            var command = new MySqlCommand(sql.ToString(), _connection);
            command.Parameters.Add($"@{NameConstants.COL_NAME}", MySqlDbType.VarChar).Value = identity.Name;
            command.Parameters.Add($"@{NameConstants.COL_EMAIL}", MySqlDbType.VarChar).Value = identity.Email;

            MySqlDataReader reader = command.ExecuteReader();

            return reader.HasRows;
        }
    }

    public IdentityDatabaseResult UpdateIdentity(UserIdentity identity)
    {
        var result = new IdentityDatabaseResult();

        try
        {
            result.Status = DatabaseResult.DatabaseResultStatus.Ok;
            result.Identity = identity;

            if (identity.Id == 0)
                throw new CriticalStateException("Tried Updating an Identity with no ID!");

            var sql = new StringBuilder();

            sql.Append($"UPDATE {NameConstants.TABLE_MAINUSER} SET ");
            sql.Append($"{NameConstants.COL_NAME} = @{NameConstants.COL_NAME}, ");
            sql.Append($"{NameConstants.COL_EMAIL} = @{NameConstants.COL_EMAIL}, ");
            sql.Append($"{NameConstants.COL_HASH} = @{NameConstants.COL_HASH}, ");
            sql.Append($"{NameConstants.COL_SALT} = @{NameConstants.COL_SALT} ");
            sql.Append(" WHERE ");
            sql.Append($"{NameConstants.COL_ID} = @{NameConstants.COL_ID};");

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql.ToString(), _connection);
                command.Parameters.Add($"@{NameConstants.COL_ID}", MySqlDbType.VarChar).Value = identity.Id;
                command.Parameters.Add($"@{NameConstants.COL_NAME}", MySqlDbType.VarChar).Value = identity.Name;
                command.Parameters.Add($"@{NameConstants.COL_EMAIL}", MySqlDbType.VarChar).Value = identity.Email;
                command.Parameters.Add($"@{NameConstants.COL_HASH}", MySqlDbType.VarChar).Value = identity.Hash;
                command.Parameters.Add($"@{NameConstants.COL_SALT}", MySqlDbType.VarChar).Value = identity.Salt;

                command.ExecuteNonQuery();
            }

            UpdateAdditionalData(identity);

            return result;
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;

            return result;
        }
    }

    private void UpdateAdditionalData(UserIdentity identity)
    {
        if (identity.Id == 0)
            throw new CriticalStateException("Tried Updating an Identity with no ID!");

        var sql = new StringBuilder();

        sql.Append($"UPDATE {NameConstants.TABLE_ADDITIONALDATA} SET ");
        sql.Append($"{NameConstants.COL_ATTEMPTS} = @{NameConstants.COL_ATTEMPTS}, ");
        sql.Append($"{NameConstants.COL_REGISTERDATE} = @{NameConstants.COL_REGISTERDATE}, ");
        sql.Append($"{NameConstants.COL_LASTLOGON} = @{NameConstants.COL_LASTLOGON}, ");
        sql.Append($"{NameConstants.COL_ITERATIONS} = @{NameConstants.COL_ITERATIONS}, ");
        sql.Append($"{NameConstants.COL_PBKDFFUNCTION} = @{NameConstants.COL_PBKDFFUNCTION}, ");
        sql.Append($"{NameConstants.COL_PASSWORDBYTELENGTH} = @{NameConstants.COL_PASSWORDBYTELENGTH}, ");
        sql.Append($"{NameConstants.COL_SALTBYTELENGTH} = @{NameConstants.COL_SALTBYTELENGTH}, ");
        sql.Append($"{NameConstants.COL_LOCKED} = @{NameConstants.COL_LOCKED}, ");
        sql.Append($"{NameConstants.COL_LOCKKEY} = @{NameConstants.COL_LOCKKEY}, ");
        sql.Append($"{NameConstants.COL_SCRYPTBLOCKSIZE} = @{NameConstants.COL_SCRYPTBLOCKSIZE}, ");
        sql.Append($"{NameConstants.COL_SCRYPTTHREADS} = @{NameConstants.COL_SCRYPTTHREADS}, ");
        sql.Append($"{NameConstants.COL_ARGONMEMORY} = @{NameConstants.COL_ARGONMEMORY}, ");
        sql.Append($"{NameConstants.COL_ARGONTHREADS} = @{NameConstants.COL_ARGONTHREADS}, ");
        sql.Append($"{NameConstants.COL_ARGONTIME} = @{NameConstants.COL_ARGONTIME}, ");
        sql.Append($"{NameConstants.COL_HASHINGFUNCTION} = @{NameConstants.COL_HASHINGFUNCTION},");
        sql.Append($"{NameConstants.COL_INTEGRITYSTATUS} = @{NameConstants.COL_INTEGRITYSTATUS} ");

        sql.Append(" WHERE ");
        sql.Append($"{NameConstants.COL_USERID} = @{NameConstants.COL_USERID};");

        using (_connection = new MySqlConnection(ConnectionString))
        {
            _connection.Open();

            var command = new MySqlCommand(sql.ToString(), _connection);

            command.Parameters.Add($"@{NameConstants.COL_USERID}", MySqlDbType.Int64).Value = identity.Id;
            command.Parameters.Add($"@{NameConstants.COL_ATTEMPTS}", MySqlDbType.Int32).Value = identity.AccountData.LoginAttempts;
            command.Parameters.Add($"@{NameConstants.COL_REGISTERDATE}", MySqlDbType.DateTime).Value = identity.AccountData.RegistrationTime;
            command.Parameters.Add($"@{NameConstants.COL_LASTLOGON}", MySqlDbType.DateTime).Value = identity.AccountData.LastLogin;
            command.Parameters.Add($"@{NameConstants.COL_ITERATIONS}", MySqlDbType.Int32).Value = identity.HashingOptions.Iterations;
            command.Parameters.Add($"@{NameConstants.COL_PBKDFFUNCTION}", MySqlDbType.Int32).Value = GetPBKDFFunctionAsInt(identity.HashingOptions);
            command.Parameters.Add($"@{NameConstants.COL_PASSWORDBYTELENGTH}", MySqlDbType.Int32).Value = identity.HashingOptions.HashByteLength;
            command.Parameters.Add($"@{NameConstants.COL_SALTBYTELENGTH}", MySqlDbType.Int32).Value = identity.HashingOptions.SaltByteLength;
            command.Parameters.Add($"@{NameConstants.COL_LOCKED}", MySqlDbType.Bit).Value = identity.AccountData.Locked; ;
            command.Parameters.Add($"@{NameConstants.COL_LOCKKEY}", MySqlDbType.VarChar).Value = identity.AccountData.LockKey;
            command.Parameters.Add($"@{NameConstants.COL_HASHINGFUNCTION}", MySqlDbType.Int32).Value = (int)identity.HashFunctionType;
            command.Parameters.Add($"@{NameConstants.COL_INTEGRITYSTATUS}", MySqlDbType.Int32).Value = (int)identity.IntegrityStatus;

            if (identity.HashingOptions != null && identity.HashingOptions is ScryptHashingOptions)
            {
                command.Parameters.Add($"@{NameConstants.COL_SCRYPTBLOCKSIZE}", MySqlDbType.Int32).Value = ((ScryptHashingOptions)identity.HashingOptions).BlockSize;
                command.Parameters.Add($"@{NameConstants.COL_SCRYPTTHREADS}", MySqlDbType.Int32).Value = ((ScryptHashingOptions)identity.HashingOptions).Threads;
            }
            else
            {
                command.Parameters.Add($"@{NameConstants.COL_SCRYPTBLOCKSIZE}", MySqlDbType.Int32).Value = 0;
                command.Parameters.Add($"@{NameConstants.COL_SCRYPTTHREADS}", MySqlDbType.Int32).Value = 0;
            }

            if (identity.HashingOptions != null && identity.HashingOptions is ArgonHashingOptions)
            {
                command.Parameters.Add($"@{NameConstants.COL_ARGONMEMORY}", MySqlDbType.Int32).Value = ((ArgonHashingOptions)identity.HashingOptions).MemoryInKB;
                command.Parameters.Add($"@{NameConstants.COL_ARGONTHREADS}", MySqlDbType.Int32).Value = ((ArgonHashingOptions)identity.HashingOptions).Threads;
                command.Parameters.Add($"@{NameConstants.COL_ARGONTIME}", MySqlDbType.Int32).Value = ((ArgonHashingOptions)identity.HashingOptions).TimeInSeconds;
            }
            else
            {
                command.Parameters.Add($"@{NameConstants.COL_ARGONMEMORY}", MySqlDbType.Int32).Value = 0;
                command.Parameters.Add($"@{NameConstants.COL_ARGONTHREADS}", MySqlDbType.Int32).Value = 0;
                command.Parameters.Add($"@{NameConstants.COL_ARGONTIME}", MySqlDbType.Int32).Value = 0;
            }

            command.ExecuteNonQuery();
        }
    }

    private int GetPBKDFFunctionAsInt(IHashingOptions options)
    {
        if (options is PBKDFHashingOptions)
            return (int)((PBKDFHashingOptions)options).HashingFunction;

        return 0;
    }

    public IdentityDatabaseResult InsertIdentity(UserIdentity identity)
    {
        var result = new IdentityDatabaseResult();

        try
        {
            result.Status = DatabaseResult.DatabaseResultStatus.Ok;
            result.Identity = identity;

            var sql = new StringBuilder();

            sql.Append($"INSERT INTO {NameConstants.TABLE_MAINUSER} ");
            sql.Append("(");
            sql.Append($"{NameConstants.COL_NAME}, ");
            sql.Append($"{NameConstants.COL_EMAIL}, ");
            sql.Append($"{NameConstants.COL_HASH}, ");
            sql.Append($"{NameConstants.COL_SALT} ");
            sql.Append(")");
            sql.Append(" VALUES ");
            sql.Append("(");
            sql.Append($"@{NameConstants.COL_NAME}, ");
            sql.Append($"@{NameConstants.COL_EMAIL}, ");
            sql.Append($"@{NameConstants.COL_HASH}, ");
            sql.Append($"@{NameConstants.COL_SALT} ");
            sql.Append(");");
            sql.Append("SELECT LAST_INSERT_ID();");

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql.ToString(), _connection);
                command.Parameters.Add($"@{NameConstants.COL_NAME}", MySqlDbType.VarChar).Value = identity.Name;
                command.Parameters.Add($"@{NameConstants.COL_EMAIL}", MySqlDbType.VarChar).Value = identity.Email;
                command.Parameters.Add($"@{NameConstants.COL_HASH}", MySqlDbType.VarChar).Value = identity.Hash;
                command.Parameters.Add($"@{NameConstants.COL_SALT}", MySqlDbType.VarChar).Value = identity.Salt;

                identity.Id = (ulong)command.ExecuteScalar();
            }

            InsertSecondary(identity);

            foreach (Role role in identity.Roles)
                InsertUserRole(identity.Id, role);

            if (_options.GenerateCustomDataTable)
                InsertCustom(identity);

            return result;
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;

            DeleteIdentity(identity);

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;

            DeleteIdentity(identity);

            return result;
        }
    }

    private void InsertUserRole(ulong userId, Roles.Role role)
    {
        var sql = new StringBuilder();

        sql.Append($"INSERT INTO {NameConstants.TABLE_ROLESUSERS} ");
        sql.Append($"({NameConstants.COL_USERID}, {NameConstants.COL_ROLEID}) VALUES ");
        sql.Append($"(@{NameConstants.COL_USERID}, @{NameConstants.COL_ROLEID});");

        using (_connection = new MySqlConnection(ConnectionString))
        {
            _connection.Open();

            var command = new MySqlCommand(sql.ToString(), _connection);

            command.Parameters.Add($"@{NameConstants.COL_USERID}", MySqlDbType.Int64).Value = userId;
            command.Parameters.Add($"@{NameConstants.COL_ROLEID}", MySqlDbType.Int64).Value = role.ValueKey;

            command.ExecuteNonQuery();
        }
    }

    private void InsertSecondary(UserIdentity identity)
    {
        var sql = new StringBuilder();

        sql.Append($"INSERT INTO {NameConstants.TABLE_ADDITIONALDATA}");
        sql.Append("(");
        sql.Append($"{NameConstants.COL_USERID}, ");
        sql.Append($"{NameConstants.COL_ATTEMPTS}, ");
        sql.Append($"{NameConstants.COL_REGISTERDATE}, ");
        sql.Append($"{NameConstants.COL_LASTLOGON}, ");
        sql.Append($"{NameConstants.COL_ITERATIONS}, ");
        sql.Append($"{NameConstants.COL_PBKDFFUNCTION}, ");
        sql.Append($"{NameConstants.COL_PASSWORDBYTELENGTH}, ");
        sql.Append($"{NameConstants.COL_SALTBYTELENGTH}, ");
        sql.Append($"{NameConstants.COL_LOCKED}, ");
        sql.Append($"{NameConstants.COL_LOCKKEY}, ");
        sql.Append($"{NameConstants.COL_SCRYPTTHREADS}, ");
        sql.Append($"{NameConstants.COL_SCRYPTBLOCKSIZE}, ");
        sql.Append($"{NameConstants.COL_ARGONMEMORY}, ");
        sql.Append($"{NameConstants.COL_ARGONTHREADS}, ");
        sql.Append($"{NameConstants.COL_ARGONTIME}, ");
        sql.Append($"{NameConstants.COL_HASHINGFUNCTION}, ");
        sql.Append($"{NameConstants.COL_INTEGRITYSTATUS} ");
        sql.Append(")");
        sql.Append(" VALUES ");
        sql.Append("(");
        sql.Append($"@{NameConstants.COL_USERID}, ");
        sql.Append($"@{NameConstants.COL_ATTEMPTS}, ");
        sql.Append($"@{NameConstants.COL_REGISTERDATE}, ");
        sql.Append($"@{NameConstants.COL_LASTLOGON}, ");
        sql.Append($"@{NameConstants.COL_ITERATIONS}, ");
        sql.Append($"@{NameConstants.COL_PBKDFFUNCTION}, ");
        sql.Append($"@{NameConstants.COL_PASSWORDBYTELENGTH}, ");
        sql.Append($"@{NameConstants.COL_SALTBYTELENGTH}, ");
        sql.Append($"@{NameConstants.COL_LOCKED}, ");
        sql.Append($"@{NameConstants.COL_LOCKKEY},");
        sql.Append($"@{NameConstants.COL_SCRYPTTHREADS},");
        sql.Append($"@{NameConstants.COL_SCRYPTBLOCKSIZE},");
        sql.Append($"@{NameConstants.COL_ARGONMEMORY},");
        sql.Append($"@{NameConstants.COL_ARGONTHREADS},");
        sql.Append($"@{NameConstants.COL_ARGONTIME},");
        sql.Append($"@{NameConstants.COL_HASHINGFUNCTION},");
        sql.Append($"@{NameConstants.COL_INTEGRITYSTATUS}");
        sql.Append(");");

        using (_connection = new MySqlConnection(ConnectionString))
        {
            //Secondary Table
            _connection.Open();

            var command = new MySqlCommand(sql.ToString(), _connection);

            command.Parameters.Add($"@{NameConstants.COL_USERID}", MySqlDbType.Int64).Value = identity.Id;
            command.Parameters.Add($"@{NameConstants.COL_ATTEMPTS}", MySqlDbType.Int32).Value = identity.AccountData.LoginAttempts ?? 0;
            command.Parameters.Add($"@{NameConstants.COL_REGISTERDATE}", MySqlDbType.DateTime).Value = DateTime.Now;
            command.Parameters.Add($"@{NameConstants.COL_LASTLOGON}", MySqlDbType.DateTime).Value = identity.AccountData.LastLogin;
            command.Parameters.Add($"@{NameConstants.COL_ITERATIONS}", MySqlDbType.Int32).Value = identity.HashingOptions.Iterations;
            command.Parameters.Add($"@{NameConstants.COL_PBKDFFUNCTION}", MySqlDbType.Int32).Value = GetPBKDFFunctionAsInt(identity.HashingOptions);
            command.Parameters.Add($"@{NameConstants.COL_PASSWORDBYTELENGTH}", MySqlDbType.Int32).Value = identity.HashingOptions.HashByteLength;
            command.Parameters.Add($"@{NameConstants.COL_SALTBYTELENGTH}", MySqlDbType.Int32).Value = identity.HashingOptions.SaltByteLength;
            command.Parameters.Add($"@{NameConstants.COL_LOCKED}", MySqlDbType.Bit).Value = identity.AccountData.Locked;
            command.Parameters.Add($"@{NameConstants.COL_LOCKKEY}", MySqlDbType.VarChar).Value = identity.AccountData.LockKey;
            command.Parameters.Add($"@{NameConstants.COL_HASHINGFUNCTION}", MySqlDbType.Int32).Value = (int)identity.HashFunctionType;
            command.Parameters.Add($"@{NameConstants.COL_INTEGRITYSTATUS}", MySqlDbType.Int32).Value = (int)identity.IntegrityStatus;

            if (identity.HashingOptions != null && identity.HashingOptions is ScryptHashingOptions)
            {
                command.Parameters.Add($"@{NameConstants.COL_SCRYPTBLOCKSIZE}", MySqlDbType.Int32).Value = ((ScryptHashingOptions)identity.HashingOptions).BlockSize;
                command.Parameters.Add($"@{NameConstants.COL_SCRYPTTHREADS}", MySqlDbType.Int32).Value = ((ScryptHashingOptions)identity.HashingOptions).Threads;
            }
            else
            {
                command.Parameters.Add($"@{NameConstants.COL_SCRYPTBLOCKSIZE}", MySqlDbType.Int32).Value = 0;
                command.Parameters.Add($"@{NameConstants.COL_SCRYPTTHREADS}", MySqlDbType.Int32).Value = 0;
            }

            if (identity.HashingOptions != null && identity.HashingOptions is ArgonHashingOptions)
            {
                command.Parameters.Add($"@{NameConstants.COL_ARGONMEMORY}", MySqlDbType.Int32).Value = ((ArgonHashingOptions)identity.HashingOptions).MemoryInKB;
                command.Parameters.Add($"@{NameConstants.COL_ARGONTHREADS}", MySqlDbType.Int32).Value = ((ArgonHashingOptions)identity.HashingOptions).Threads;
                command.Parameters.Add($"@{NameConstants.COL_ARGONTIME}", MySqlDbType.Int32).Value = ((ArgonHashingOptions)identity.HashingOptions).TimeInSeconds;

            }
            else
            {
                command.Parameters.Add($"@{NameConstants.COL_ARGONMEMORY}", MySqlDbType.Int32).Value = 0;
                command.Parameters.Add($"@{NameConstants.COL_ARGONTHREADS}", MySqlDbType.Int32).Value = 0;
                command.Parameters.Add($"@{NameConstants.COL_ARGONTIME}", MySqlDbType.Int32).Value = 0;
            }

            command.ExecuteScalar();
        }
    }

    private void InsertCustom(UserIdentity identity)
    {
        if (identity.Id <= 0)
            return;

        var sql = new StringBuilder();

        sql.Append($"INSERT INTO {NameConstants.TABLE_CUSTOMDATA} ");
        sql.Append("(");
        sql.Append($"{NameConstants.COL_USERID}");
        sql.Append(")");
        sql.Append(" VALUES ");
        sql.Append("(");
        sql.Append($"@{NameConstants.COL_USERID}");
        sql.Append(");");

        using (_connection = new MySqlConnection(ConnectionString))
        {
            _connection.Open();

            var command = new MySqlCommand(sql.ToString(), _connection);

            command.Parameters.Add($"@{NameConstants.COL_USERID}", MySqlDbType.Int64).Value = identity.Id;

            command.ExecuteScalar();
        }
    }

    public IdentityDatabaseResult DeleteIdentity(UserIdentity identity)
    {
        var result = new IdentityDatabaseResult();

        try
        {
            if (identity.Id <= 0)
                throw new ArgumentException("Identity.Id <= 0");

            result.Status = DatabaseResult.DatabaseResultStatus.Ok;
            result.Identity = identity;

            var sql = new StringBuilder();

            sql.Append($"DELETE FROM {NameConstants.TABLE_CUSTOMDATA} WHERE {NameConstants.COL_USERID} = @{NameConstants.COL_ID};");
            sql.Append($"DELETE FROM {NameConstants.TABLE_ADDITIONALDATA} WHERE {NameConstants.COL_USERID} = @{NameConstants.COL_ID};");
            sql.Append($"DELETE FROM {NameConstants.TABLE_MAINUSER} WHERE {NameConstants.COL_ID} = @{NameConstants.COL_ID};");

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql.ToString(), _connection);

                command.Parameters.Add($"@{NameConstants.COL_ID}", MySqlDbType.Int64).Value = identity.Id;

                command.ExecuteNonQuery();
            }

            return result;
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;

            return result;
        }
    }

    public IdentitiesDatabaseResult GetIdentities()
    {
        var result = new IdentitiesDatabaseResult();

        try
        {
            result.Status = DatabaseResult.DatabaseResultStatus.Ok;
            result.Identities = new List<UserIdentity>();

            var sql = new StringBuilder();

            sql.Append($"SELECT * FROM {NameConstants.TABLE_MAINUSER} MAIN_TABLE ");
            sql.Append($"INNER JOIN {NameConstants.TABLE_ADDITIONALDATA} ADDITIONAL_TABLE ");
            sql.Append($"ON ADDITIONAL_TABLE.{NameConstants.COL_USERID} = MAIN_TABLE.{NameConstants.COL_ID};");

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql.ToString(), _connection);
                var dataTable = new DataTable();

                MySqlDataReader reader = command.ExecuteReader();

                dataTable.Load(reader);

                foreach (DataRow row in dataTable.Rows)
                {
                    var identity = new UserIdentity(row);
                    identity.Roles = GetRoles(identity.Id);

                    result.Identities.Add(identity);
                }

                return result;
            }
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;

            return result;
        }
    }

    private List<Role> GetRoles(ulong id)
    {
        var sql = new StringBuilder();

        sql.Append($"SELECT * FROM {NameConstants.TABLE_ROLESUSERS} ASSIGNMENTTABLE ");
        sql.Append($"INNER JOIN {NameConstants.TABLE_ROLES} ROLES ");
        sql.Append($"ON ASSIGNMENTTABLE.{NameConstants.COL_ROLEID} = ROLES.{NameConstants.COL_ROLENUMERICVALUE} ");
        sql.Append($"WHERE ASSIGNMENTTABLE.{NameConstants.COL_USERID} = @{NameConstants.COL_USERID};");

        using (_connection = new MySqlConnection(ConnectionString))
        {
            _connection.Open();

            var command = new MySqlCommand(sql.ToString(), _connection);
            command.Parameters.Add($"@{NameConstants.COL_USERID}", MySqlDbType.Int64).Value = id;

            var dataTable = new DataTable();

            MySqlDataReader reader = command.ExecuteReader();

            dataTable.Load(reader);

            var roles = new List<Role>();

            foreach (DataRow row in dataTable.Rows)
            {
                roles.Add(new Role(row));
            }

            return roles;
        }
    }

    public IdentityDatabaseResult GetIdentity(string? username, string? email)
    {
        return GetIdentity(username, email, null);
    }

    public IdentityDatabaseResult GetIdentity(ulong Id)
    {
        return GetIdentity(String.Empty, String.Empty, Id);
    }

    private IdentityDatabaseResult GetIdentity(string? username, string? email, ulong? Id)
    {
        var result = new IdentityDatabaseResult();

        try
        {
            result.Status = DatabaseResult.DatabaseResultStatus.Ok;

            var sql = new StringBuilder();

            sql.Append($"SELECT * FROM {NameConstants.TABLE_MAINUSER} MAIN_TABLE ");
            sql.Append($"INNER JOIN {NameConstants.TABLE_ADDITIONALDATA} ADDITIONAL_TABLE ");
            sql.Append($"ON ADDITIONAL_TABLE.{NameConstants.COL_USERID} = MAIN_TABLE.{NameConstants.COL_ID}");
            sql.Append(" WHERE ");

            if (Id == null)
            {
                if (!String.IsNullOrEmpty(username) && !String.IsNullOrEmpty(email))
                    sql.Append($"(MAIN_TABLE.{NameConstants.COL_NAME} = @{NameConstants.COL_NAME} AND MAIN_TABLE.{NameConstants.COL_EMAIL} = @{NameConstants.COL_EMAIL});");
                else if (!String.IsNullOrEmpty(email))
                    sql.Append($"MAIN_TABLE.{NameConstants.COL_EMAIL} = @{NameConstants.COL_EMAIL};");
                else if (!String.IsNullOrEmpty(username))
                    sql.Append($"MAIN_TABLE.{NameConstants.COL_NAME} = @{NameConstants.COL_NAME};");
            }
            else
            {
                sql.Append($"MAIN_TABLE.{NameConstants.COL_ID} = @{NameConstants.COL_ID};");
            }

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql.ToString(), _connection);

                if (Id == null)
                {
                    if (!String.IsNullOrEmpty(username))
                        command.Parameters.Add($"@{NameConstants.COL_NAME}", MySqlDbType.VarChar).Value = username;

                    if (!String.IsNullOrEmpty(email))
                        command.Parameters.Add($"@{NameConstants.COL_EMAIL}", MySqlDbType.VarChar).Value = email;
                }
                else
                {
                    command.Parameters.Add($"@{NameConstants.COL_ID}", MySqlDbType.Int64).Value = Id;
                }

                MySqlDataReader reader = command.ExecuteReader();

                var dataTable = new DataTable();

                dataTable.Load(reader);

                if (dataTable.Rows.Count == 0)
                    throw new IdentityDoesNotExistException();

                if (dataTable.Rows.Count > 1)
                    throw new CriticalStateException("Multiple Users with same name or email found.");

                result.Identity = new UserIdentity(dataTable.Rows[0]);
                result.Identity.Roles = GetRoles(result.Identity.Id);

                return result;
            }
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;

            return result;
        }
    }

    public RolesDatabaseResult GetAllRoles()
    {
        var result = new RolesDatabaseResult();
        result.Status = DatabaseResult.DatabaseResultStatus.Ok;
        result.Roles = new List<Role>();

        try
        {
            string sql = $"SELECT {NameConstants.COL_ROLENAME},{NameConstants.COL_ROLENUMERICVALUE} FROM {NameConstants.TABLE_ROLES};";

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql, _connection);
                var dataTable = new DataTable();

                MySqlDataReader reader = command.ExecuteReader();

                dataTable.Load(reader);

                result.Roles = new List<Role>();

                foreach (DataRow row in dataTable.Rows)
                {
                    result.Roles.Add(new Role(row));
                }

                return result;
            }
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;
            result.Roles = null;

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;
            result.Roles = null;

            return result;
        }
    }

    public RoleDatabaseResult InsertRole(Role role)
    {
        var result = new RoleDatabaseResult();
        result.Status = DatabaseResult.DatabaseResultStatus.Ok;

        try
        {
            var sql = new StringBuilder();

            sql.Append($"INSERT INTO {NameConstants.TABLE_ROLES} ");
            sql.Append($"({NameConstants.COL_ROLENUMERICVALUE}, {NameConstants.COL_ROLENAME})");
            sql.Append($" VALUES ");
            sql.Append($"(@{NameConstants.COL_ROLENUMERICVALUE}, @{NameConstants.COL_ROLENAME});");

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql.ToString(), _connection);

                command.Parameters.Add($"@{NameConstants.COL_ROLENAME}", MySqlDbType.VarChar).Value = role.Name;
                command.Parameters.Add($"@{NameConstants.COL_ROLENUMERICVALUE}", MySqlDbType.Int32).Value = role.ValueKey;

                command.ExecuteNonQuery();

                result.Role = role;

                return result;
            }
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;
            result.Role = null;

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;
            result.Role = null;

            return result;
        }
    }

    public RolesDatabaseResult DeleteAllRoles()
    {
        var result = new RolesDatabaseResult();
        result.Status = DatabaseResult.DatabaseResultStatus.Ok;
        result.Roles = new List<Role>();

        try
        {
            string sql = $"DELETE FROM {NameConstants.TABLE_ROLES}";

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql, _connection);

                command.ExecuteNonQuery();

                return result;
            }
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;
            result.Roles = null;

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;
            result.Roles = null;

            return result;
        }
    }

    public RolesDatabaseResult DeleteObsoleteRoleAssignments()
    {
        var result = new RolesDatabaseResult();
        result.Status = DatabaseResult.DatabaseResultStatus.Ok;
        result.Roles = new List<Role>();

        try
        {
            string sql = $"DELETE FROM {NameConstants.TABLE_ROLESUSERS} WHERE {NameConstants.COL_ROLEID} NOT IN (SELECT {NameConstants.COL_ROLENUMERICVALUE} FROM {NameConstants.TABLE_ROLES});";

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql, _connection);

                command.ExecuteNonQuery();

                return result;
            }
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;
            result.Roles = null;

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;
            result.Roles = null;

            return result;
        }
    }

    public IdentityDatabaseResult InsertIdentityRole(ulong userId, Roles.Role role)
    {
        var result = new IdentityDatabaseResult();
        result.Status = DatabaseResult.DatabaseResultStatus.Ok;

        try
        {
            var sql = new StringBuilder();

            sql.Append($"INSERT INTO {NameConstants.TABLE_ROLESUSERS} ");
            sql.Append($"({NameConstants.COL_ROLEID}, {NameConstants.COL_USERID})");
            sql.Append($" VALUES ");
            sql.Append($"(@{NameConstants.COL_ROLEID}, @{NameConstants.COL_USERID});");

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql.ToString(), _connection);

                command.Parameters.Add($"@{NameConstants.COL_ROLEID}", MySqlDbType.Int64).Value = role.ValueKey;
                command.Parameters.Add($"@{NameConstants.COL_USERID}", MySqlDbType.Int64).Value = userId;

                command.ExecuteNonQuery();

                return result;
            }
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;
            result.Identity = null;

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;
            result.Identity = null;

            return result;
        }
    }

    public IdentityDatabaseResult DeleteIdentityRoles(UserIdentity identity)
    {
        var result = new IdentityDatabaseResult();
        result.Status = DatabaseResult.DatabaseResultStatus.Ok;

        try
        {
            var sql = new StringBuilder();

            sql.Append($"DELETE FROM {NameConstants.TABLE_ROLESUSERS} ");
            sql.Append($"WHERE {NameConstants.COL_USERID} = @{NameConstants.COL_USERID};");

            using (_connection = new MySqlConnection(ConnectionString))
            {
                _connection.Open();

                var command = new MySqlCommand(sql.ToString(), _connection);

                command.Parameters.Add($"@{NameConstants.COL_USERID}", MySqlDbType.Int64).Value = identity.Id;

                command.ExecuteNonQuery();

                return result;
            }
        }
        catch (MySqlException e)
        {
            result.ThrownException = e;
            result.Status = DatabaseResult.DatabaseResultStatus.DbError;
            result.Identity = null;

            return result;
        }
        catch (Exception ex)
        {
            result.ThrownException = ex;
            result.Status = DatabaseResult.DatabaseResultStatus.CodeError;
            result.Identity = null;

            return result;
        }
    }
}