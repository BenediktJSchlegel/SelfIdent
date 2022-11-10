using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SelfIdent.Account.SecurityContext;

namespace SelfIdent.Interfaces;

internal interface ISecurityContextHandler
{
    CookieContextResult AuthenticateCookie(CookieContextPayload payload);
    Task<CookieContextResult> AuthenticateCookieAsync(CookieContextPayload payload);
    TokenContextResult AuthenticateToken(TokenContextPayload payload);
    void LogoutCookie(HttpContext context);
    Task LogoutCookieAsync(CookieContextPayload payload);
    TokenValidationResult ValidateToken(TokenValidationPayload payload);
}
