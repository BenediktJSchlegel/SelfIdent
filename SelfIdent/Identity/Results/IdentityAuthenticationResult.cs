using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Identity.Results;

internal class IdentityAuthenticationResult
{
    public bool Successful { get; set; } = true;
    public UserIdentity? AuthenticatedIdentity { get; set; }
    public Exception? ThrownException { get; set; }
    public bool UserExists { get; set; }
}
