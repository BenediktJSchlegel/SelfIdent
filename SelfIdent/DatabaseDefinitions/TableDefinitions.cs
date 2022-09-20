using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Constants;
using SelfIdent.Options;

namespace SelfIdent.DatabaseDefinitions;

internal static class TableDefinitions
{
    public static List<TableDefinition> GetTableDefinitions(SelfIdentOptions options)
    {
        List<TableDefinition> results = new List<TableDefinition>();

        results.Add(InstantiateMainUserTable());
        results.Add(InstantiateAdditionalDataTable());
        results.Add(InstantiateRoleTable());
        results.Add(InstantiateRoleUserTable());

        if (options.GenerateCustomDataTable)
            results.Add(InstantiateCustomDataTable());

        if (options.MultiFactorAuthenticationActive)
            results.Add(InstantiateMFATable());

        return results;
    }

    private static TableDefinition InstantiateRoleTable()
    {
        TableDefinition table = new TableDefinition(NameConstants.TABLE_ROLES);

        table.Columns = new List<TableColumn>();

        table.Columns.Add(new TableColumn(NameConstants.COL_ID, DataTypes.Long, null, false, 0, true, true));
        table.Columns.Add(new TableColumn(NameConstants.COL_ROLENUMERICVALUE, DataTypes.Int, 255, false, 1, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_ROLENAME, DataTypes.String, 255, false, 2, false));

        return table;
    }

    private static TableDefinition InstantiateRoleUserTable()
    {
        TableDefinition table = new TableDefinition(NameConstants.TABLE_ROLESUSERS);

        table.Columns = new List<TableColumn>();

        table.Columns.Add(new TableColumn(NameConstants.COL_USERID, DataTypes.Long, null, false, 0, true, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_ROLEID, DataTypes.Long, null, false, 1, true, false));

        return table;
    }

    private static TableDefinition InstantiateMFATable()
    {
        TableDefinition table = new TableDefinition(NameConstants.TABLE_MFA);

        table.Columns = new List<TableColumn>();

        table.Columns.Add(new TableColumn(NameConstants.COL_ID, DataTypes.Long, null, false, 0, true, true));
        table.Columns.Add(new TableColumn(NameConstants.COL_USERID, DataTypes.Long, null, false, 1, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_SESSIONID, DataTypes.String, 255, false, 2, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_MFAKEY, DataTypes.String, 255, false, 3, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_TIMEISSUED, DataTypes.DateTime, null, false, 4, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_TIMEEXPIRED, DataTypes.DateTime, null, false, 5, false));

        return table;
    }

    private static TableDefinition InstantiateMainUserTable()
    {
        TableDefinition table = new TableDefinition(NameConstants.TABLE_MAINUSER);

        table.Columns = new List<TableColumn>();

        table.Columns.Add(new TableColumn(NameConstants.COL_ID, DataTypes.Long, null, false, 0, true, true));
        table.Columns.Add(new TableColumn(NameConstants.COL_NAME, DataTypes.String, 255, false, 1, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_EMAIL, DataTypes.String, 255, false, 2, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_HASH, DataTypes.String, 255, false, 3, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_SALT, DataTypes.String, 255, false, 4, false));

        return table;
    }

    private static TableDefinition InstantiateAdditionalDataTable()
    {
        TableDefinition table = new TableDefinition(NameConstants.TABLE_ADDITIONALDATA);

        table.Columns = new List<TableColumn>();

        table.Columns.Add(new TableColumn(NameConstants.COL_ID, DataTypes.Long, null, false, 0, true, true));
        table.Columns.Add(new TableColumn(NameConstants.COL_USERID, DataTypes.Long, null, false, 1, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_ATTEMPTS, DataTypes.Int, null, false, 2, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_LOCKED, DataTypes.Bool, null, false, 3, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_LOCKKEY, DataTypes.String, 255, true, 4, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_LASTLOGON, DataTypes.DateTime, null, true, 5, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_REGISTERDATE, DataTypes.DateTime, null, false, 6, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_ITERATIONS, DataTypes.Int, null, false, 7, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_PBKDFFUNCTION, DataTypes.Int, 255, false, 8, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_PASSWORDBYTELENGTH, DataTypes.Int, 255, false, 9, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_SALTBYTELENGTH, DataTypes.Int, 255, false, 10, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_HASHINGFUNCTION, DataTypes.Int, 255, false, 11, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_SCRYPTBLOCKSIZE, DataTypes.Int, 255, false, 12, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_SCRYPTTHREADS, DataTypes.Int, 255, false, 13, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_ARGONMEMORY, DataTypes.Int, 255, false, 14, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_ARGONTHREADS, DataTypes.Int, 255, false, 15, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_ARGONTIME, DataTypes.Int, 255, false, 16, false));
        table.Columns.Add(new TableColumn(NameConstants.COL_INTEGRITYSTATUS, DataTypes.Int, null, false, 17, false));

        return table;
    }

    private static TableDefinition InstantiateCustomDataTable()
    {
        TableDefinition table = new TableDefinition(NameConstants.TABLE_CUSTOMDATA);

        table.Columns = new List<TableColumn>();

        table.Columns.Add(new TableColumn(NameConstants.COL_ID, DataTypes.Long, null, false, 0, true, true));
        table.Columns.Add(new TableColumn(NameConstants.COL_USERID, DataTypes.Long, null, false, 1, false));

        return table;
    }
}

internal class TableDefinition
{
    public string Name { get; set; }
    public List<TableColumn> Columns { get; set; }

    public TableDefinition(string name)
    {
        this.Name = name;
        this.Columns = new List<TableColumn>();
    }
}
