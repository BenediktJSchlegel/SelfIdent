using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Identity.Results;

internal class IdentityCreationResult
{
    public bool Successful { get; set; } = true;
    public UserIdentity? CreatedIdentity { get; set; }
    public Exception? ThrownException { get; set; }
}
