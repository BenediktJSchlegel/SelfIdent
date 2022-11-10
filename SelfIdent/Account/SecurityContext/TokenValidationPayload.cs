using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Account.SecurityContext;

public class TokenValidationPayload
{
    public string AuthenticationToken { get; set; }

    public TokenValidationPayload(string token)
    {
        this.AuthenticationToken = token;
    }
}
