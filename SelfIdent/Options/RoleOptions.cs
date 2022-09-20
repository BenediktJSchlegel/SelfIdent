using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Options;

public class RoleOptions
{
    /// <summary>
    /// List of all Roles available. 
    /// Removing a Role from this List after the Database was already generated will remove that option
    /// from the Database and removes that role from all users that have it.
    /// -
    /// Roles are identified by their OrderKey, not their Name.
    /// If the Name for a certain OrderKey is changed the Name of the Role will be updated.
    /// All Users with that Role will keep it.
    /// </summary>
    public List<Roles.Role> Roles { get; set; }
    /// <summary>
    /// List of all Roles that are added to new Users by default
    /// </summary>
    public List<Roles.Role> DefaultRoles { get; set; }

    public RoleOptions(List<Roles.Role> roles, List<Roles.Role> defaults)
    {
        this.Roles = roles;
        this.DefaultRoles = defaults;
    }

    public RoleOptions()
    {
        this.Roles = new List<Roles.Role>();
        this.DefaultRoles = new List<Roles.Role>();
    }
}
