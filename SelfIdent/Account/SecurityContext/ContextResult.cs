using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Account.SecurityContext;

internal abstract class ContextResult
{
    public bool Successful { get; set; } = true;
    public Exception? ThrownException { get; set; }
}
