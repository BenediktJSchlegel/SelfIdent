using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Constants;
using SelfIdent.Helpers;

namespace SelfIdent.Roles;

public class Role
{
    public Role(int key, string name)
    {
        this.ValueKey = key;
        this.Name = name;
    }

    public Role(DataRow row)
    {
        this.ValueKey = Helper.SafelySet<int>(row[NameConstants.COL_ROLENUMERICVALUE]);
        this.Name = Helper.SafelySet<string>(row[NameConstants.COL_ROLENAME]) ?? String.Empty;
    }

    /// <summary>
    /// The Numeric Value of this Role.
    /// This is what a Role is identified by within the package
    /// </summary>
    public int ValueKey { get; set; }
    /// <summary>
    /// The Name of the Role.
    /// </summary>
    public string Name { get; set; }

    public Role Clone()
    {
        return new Role(this.ValueKey, this.Name);
    }

    public static bool RolesMatch(List<Role> first, List<Role> second)
    {
        if (first == null || second == null)
        {
            if ((first == null && second != null) || (second == null && first != null))
                return false;

            return true;
        }
        else
        {
            if (first.Count != second.Count)
                return false;

            first = first.OrderBy(r => r.ValueKey).ToList();
            second = second.OrderBy(r => r.ValueKey).ToList();

            for (int i = 0; i < first.Count; i++)
            {
                if (first[i].Name != second[i].Name || first[i].ValueKey != second[i].ValueKey)
                    return false;
            }
        }

        return true;
    }

}
