using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Identity.Results;

internal class IdentityDeletionResult
{
    public bool Successful { get; set; } = true;
    public UserIdentity? DeletedIdentity { get; set; }
    public Exception? ThrownException { get; set; }
}
