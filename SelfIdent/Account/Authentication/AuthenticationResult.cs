using Microsoft.AspNetCore.Authentication;
using SelfIdent.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace SelfIdent.Account.Authentication;

public enum AuthenticationResultStatus { OK, FAILED_UNKNOWN, FAILED_LOCKED, FAILED_BADINPUT, FAILED_UNKNOWNUSER }
/// <summary>
/// Gets returned when an Authentication attempt concludes
/// </summary>
public class AuthenticationResult : IEndpointResult
{
    public bool Successful { get; set; }
    public User? User { get; set; }
    public Exception? ThrownException { get; set; }
    public AuthenticationResultStatus Status { get; set; }
}

public class CookieAuthenticationResult : AuthenticationResult
{
    /// <summary>
    /// ClaimsPrincipal to use for Cookie Authentication of .Net
    /// </summary>
    public ClaimsPrincipal? CookieClaimsPrincipal { get; set; }
    /// <summary>
    /// AuthenticationProperties for Cookie Authentication of .Net
    /// </summary>
    public AuthenticationProperties? AuthenticationProperties { get; set; }

}

public class TokenAuthenticationResult : AuthenticationResult
{
    /// <summary>
    /// The Generated Token
    /// </summary>
    public Token.AuthenticationToken? Token { get; set; }

    public TokenAuthenticationResult()
    {
        //
    }

    public TokenAuthenticationResult(AuthenticationResult result)
    {
        this.Status = result.Status;
        this.Successful = result.Successful;
        this.ThrownException = result.ThrownException;
        this.User = result.User;
    }
}
