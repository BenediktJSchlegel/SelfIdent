using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Account.SecurityContext;

internal class TokenValidationResult
{
    public string AuthenticationToken { get; set; }
    public ulong UserId { get; set; }
    public bool Successful { get; set; }
    public Exception? ThrownException { get; set; }

    public TokenValidationResult(string token)
    {
        this.AuthenticationToken = token;
    }
}

public class TokenUserValidationResult
{
    public string AuthenticationToken { get; set; }
    public User? User { get; set; }
    public bool Successful { get; set; }
    public Exception? ThrownException { get; set; }

    public TokenUserValidationResult(string token)
    {
        this.AuthenticationToken = token;
    }
}