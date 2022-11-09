using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SelfIdent.Cache;
using SelfIdent.Enums;
using SelfIdent.Helpers;
using SelfIdent.Identity;
using SelfIdent.Interfaces;

namespace SelfIdent.AuthenticationEvents;

public class SelfIdentCookieAuthenticationEvents : CookieAuthenticationEvents
{
    //https://docs.microsoft.com/en-us/aspnet/core/security/authentication/cookie?view=aspnetcore-5.0

    private IdentityCache _cache;
    private SelfIdent _selfIdent;

    public SelfIdentCookieAuthenticationEvents(SelfIdent selfIdent)
    {
        _selfIdent = selfIdent;
        _cache = new IdentityCache(selfIdent.GetDatabaseService(), selfIdent.MemoryCache);
    }

    public override Task ValidatePrincipal(CookieValidatePrincipalContext context)
    {
        try
        {
            if (context == null || context.Principal == null)
                throw new ArgumentException("No SecurityContext.");

            ulong? userId = SecurityContextHelper.GetClaimValue<ulong>(context.Principal, Constants.MainConstants.CLAIM_ID);

            if (userId == null)
                throw new Exception("No ID Claim.");

            UserIdentity? identity = _cache.GetIdentity((ulong)userId);

            if (identity == null || identity.AccountData.Locked || !SecurityContextHelper.ClaimsAreValid(context.Principal, identity))
            {
                SecurityContextHelper.DestroySecurityContext(context);
                return Task.CompletedTask;
            }

            if (identity.IntegrityStatus == SecurityContextIntegrityStatus.UPDATE)
                context.ReplacePrincipal(GenerateNewPrincipal(identity));
            else if (identity.IntegrityStatus == SecurityContextIntegrityStatus.INVALID)
                SecurityContextHelper.DestroySecurityContext(context);
        }
        catch (Exception)
        {
            SecurityContextHelper.DestroySecurityContext(context);
        }

        return Task.CompletedTask;
    }

    private ClaimsPrincipal GenerateNewPrincipal(UserIdentity identity)
    {
        var primaryIdentity = new ClaimsIdentity(identity.ToPublicUser().ToClaims(), _selfIdent.Options.SecurityContextOptions.AuthenticationSchema);
        var claimsPrincipal = new ClaimsPrincipal(primaryIdentity);

        return claimsPrincipal;
    }

}
