using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Identity;

internal class IdentityAuthenticationStatus
{
    /// <summary>
    /// If user has been successfully authenticated
    /// </summary>
    public bool Authenticated { get; set; } = false;
}
