using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SelfIdent.Account.SecurityContext;

internal class TokenContextPayload : ContextPayload
{
    public TokenContextPayload(HttpContext context, User user) : base(context, user)
    {

    }
}
