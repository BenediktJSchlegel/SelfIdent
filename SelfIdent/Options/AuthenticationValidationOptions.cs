using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Options;

public abstract class AuthenticationValidationOptions
{

}

public class TokenAuthenticationValidationOptions : AuthenticationValidationOptions
{
    public string SecretKey { get; set; }

    public TokenAuthenticationValidationOptions(string secretKey)
    {
        this.SecretKey = secretKey;
    }
}

public class CookieAuthenticationValidationOptions : AuthenticationValidationOptions
{
    // TODO:
}

