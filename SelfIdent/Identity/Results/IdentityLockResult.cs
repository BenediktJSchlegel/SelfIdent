using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Identity.Results;

internal class IdentityLockResult
{
    public bool Successful { get; set; } = true;
    public UserIdentity? LockedIdentity { get; set; }
    public string? GeneratedKey { get; set; }
    public Exception? ThrownException { get; set; }
}
