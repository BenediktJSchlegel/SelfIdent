using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;
using SelfIdent.Identity;
using SelfIdent.Options;

namespace SelfIdent.Token
{
    /// <summary>
    /// https://jasonwatmore.com/post/2021/06/02/net-5-create-and-validate-jwt-tokens-use-custom-jwt-middleware
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SelfIdentTokenAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private const string UserKey = "User";

        /// <summary>
        /// Roles that must be assigned to the user for authentication to succeed
        /// </summary>
        public IEnumerable<string>? Roles { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (AllowsAnonymous(context))
                return;

            if (context.HttpContext.Items.ContainsKey(UserKey) && context.HttpContext.Items[UserKey] is UserIdentity identity)
            {
                // User exists. Validate roles if any are specified
                if (Roles != null)
                {
                    if (!RolesAreValid(identity, Roles))
                        context.Result = new JsonResult(new { message = "Unauthorized - Missing role" }) { StatusCode = StatusCodes.Status401Unauthorized };
                }
            }
            else
            {
                // No user set. Can not be authorized
                context.Result = new JsonResult(new { message = "Unauthorized - No user specified" }) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }

        private bool RolesAreValid(UserIdentity identity, IEnumerable<string> roles)
        {
            foreach (string role in roles)
            {
                if (identity.Roles.FirstOrDefault(r => r.Name == role) == null)
                    return false;
            }

            return true;
        }

        private bool AllowsAnonymous(AuthorizationFilterContext context)
        {
            return context.ActionDescriptor.EndpointMetadata.OfType<SelfIdentAllowAnonymousAttribute>().Any();
        }

    }
}
