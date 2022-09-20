using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using SelfIdent.Identity;

namespace SelfIdent.AuthenticationEvents;

internal static class SecurityContextHelper
{
    internal static bool ClaimsAreValid(ClaimsPrincipal? principal, UserIdentity identity)
    {
        if (identity == null || principal == null)
            return false;

        return true;
    }

    internal static T? GetClaimValue<T>(ClaimsPrincipal principal, string claimType) where T : struct
    {
        Claim? claim = principal.FindFirst(claimType);

        if (claim == null)
            return null;

        object? convertedValue = Convert.ChangeType(claim.Value, typeof(T));

        if (convertedValue == null || !(convertedValue is T))
            return null;

        return (T)convertedValue;
    }

    internal static void DestroySecurityContext(CookieValidatePrincipalContext context)
    {
        context.RejectPrincipal();
        context.HttpContext.SignOutAsync().Wait();
    }
}
