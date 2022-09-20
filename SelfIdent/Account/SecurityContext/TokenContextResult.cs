using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Token;

namespace SelfIdent.Account.SecurityContext;

internal class TokenContextResult : ContextResult
{
    public AuthenticationToken? Token { get; set; }
}
