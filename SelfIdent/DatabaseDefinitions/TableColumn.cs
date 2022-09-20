using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.DatabaseDefinitions;

class TableColumn
{
    /// <summary>
    /// Name of the Column
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Database Type => NOT Service specific
    /// </summary>
    public DataTypes Type { get; set; }
    /// <summary>
    /// Length of the Column
    /// Null = Default
    /// </summary>
    public int? Length { get; set; }
    /// <summary>
    /// If DB value can be null
    /// </summary>
    public bool Nullable { get; set; }
    /// <summary>
    /// What position the Column is supposed to be at
    /// </summary>
    public int Position { get; set; }
    /// <summary>
    /// If the Col is the primary key
    /// </summary>
    public bool IsPrimaryKey { get; set; }
    /// <summary>
    /// If field should be automatically incremented
    /// </summary>
    public bool Autoincrement { get; set; }

    public TableColumn(string name, DataTypes type, int? length, bool nullable, int position, bool autoincrement, bool primaryKey = false)
    {
        Name = name;
        Type = type;
        Length = length;
        Nullable = nullable;
        Position = position;
        IsPrimaryKey = primaryKey;
        Autoincrement = autoincrement;
    }
}
