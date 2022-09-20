using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SelfIdent.Account.SecurityContext;
using SelfIdent.Constants;
using SelfIdent.Helpers;
using SelfIdent.Interfaces;
using SelfIdent.Options;

namespace SelfIdent.SecurityContext;

internal class SecurityContextHandler : ISecurityContextHandler
{
    private SecurityContextOptions _options;

    public SecurityContextHandler(SecurityContextOptions options)
    {
        _options = options;
    }

    public Task<CookieContextResult> AuthenticateCookieAsync(CookieContextPayload payload)
    {
        var result = new CookieContextResult();
        result.Successful = true;

        try
        {
            if (payload.User == null)
                throw new ArgumentException("No User given to Authenticate");

            var primaryIdentity = new ClaimsIdentity(Helper.GetClaims(payload.User), _options.AuthenticationSchema);
            var claimsPrincipal = new ClaimsPrincipal(primaryIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = payload.PersistentCookie,
                IssuedUtc = DateTime.UtcNow,
                RedirectUri = payload.RedirectUri,
                AllowRefresh = payload.AllowRefresh,
                ExpiresUtc = payload.ExpiresUtc,
            };

            payload.Context.SignInAsync(_options.AuthenticationSchema, claimsPrincipal, authProperties);

            return Task.FromResult(result);
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.ThrownException = e;

            return Task.FromResult(result);
        }
    }

    public CookieContextResult AuthenticateCookie(CookieContextPayload payload)
    {
        var result = new CookieContextResult();
        result.Successful = true;

        try
        {
            if (payload.User == null)
                throw new ArgumentException("No User given for Authentication.");

            var primaryIdentity = new ClaimsIdentity(Helper.GetClaims(payload.User), _options.AuthenticationSchema);
            var claimsPrincipal = new ClaimsPrincipal(primaryIdentity);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = payload.PersistentCookie,
                IssuedUtc = DateTime.UtcNow,
                RedirectUri = payload.RedirectUri,
                AllowRefresh = payload.AllowRefresh,
                ExpiresUtc = payload.ExpiresUtc,
            };

            payload.Context.SignInAsync(_options.AuthenticationSchema, claimsPrincipal, authProperties).Wait();

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.ThrownException = e;

            return result;
        }
    }

    public TokenContextResult AuthenticateToken(TokenContextPayload payload)
    {
        var result = new TokenContextResult();
        result.Successful = true;

        try
        {
            var authToken = new Token.AuthenticationToken();

            if (String.IsNullOrWhiteSpace(_options.TokenSecretKey))
                throw new ArgumentException("No TokenSecretKey given while trying to sign a Token.");

            DateTime expiresAt = GetTokenExpirationDate(_options.TokenLifetime);
            byte[] keyBytes = Encoding.ASCII.GetBytes(_options.TokenSecretKey);
            var symmetricKey = new SymmetricSecurityKey(keyBytes);

            var token = new JwtSecurityToken(
                            claims: Helper.GetClaims(payload.User),
                            notBefore: DateTime.UtcNow,
                            expires: expiresAt,
                            signingCredentials: new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256Signature)
                            );

            authToken.AccessToken = new JwtSecurityTokenHandler().WriteToken(token);
            authToken.ExpiresAt = expiresAt;

            result.Token = authToken;

            return result;
        }
        catch (Exception e)
        {
            result.Successful = false;
            result.ThrownException = e;
            result.Token = null;

            return result;
        }
    }

    public void LogoutCookie(HttpContext context)
    {
        context.SignOutAsync(_options.AuthenticationSchema).Wait();
    }

    public Task LogoutCookieAsync(CookieContextPayload payload)
    {
        return payload.Context.SignOutAsync(_options.AuthenticationSchema);
    }

    private DateTime GetTokenExpirationDate(TimeSpan duration)
    {
        return DateTime.UtcNow + duration;
    }

}
