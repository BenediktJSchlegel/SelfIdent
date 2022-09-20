using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.IdentityModel.Tokens;

namespace SelfIdent.Token
{
    public class SelfIdentTokenMiddleware
    {
        private readonly RequestDelegate _next;

        public SelfIdentTokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, SelfIdent instance) 
        {
            if (String.IsNullOrEmpty(instance.Options.SecurityContextOptions.TokenSecretKey))
                return;


            string headerValue = GetAuthorizationHeaderValue(context);

            if (String.IsNullOrEmpty(headerValue))
                return;


            JwtSecurityToken? token = GetToken(headerValue, instance.Options.SecurityContextOptions.TokenSecretKey);

            if (token == null)
                return;


            User? user = instance.GetUser(GetIdClaimValue(token));

            if (user == null || user.Locked)
                return;


            context.Items["User"] = user;

            await _next(context);
        }


        private UInt64 GetIdClaimValue(JwtSecurityToken token)
        {
            return UInt64.Parse(token.Claims.First(x => x.Type == "Id").Value);
        }

        private string GetAuthorizationHeaderValue(HttpContext context)
        {
            if (context != null && context.Request != null && context.Request.Headers != null && context.Request.Headers.ContainsKey("Authorization"))
                return context.Request.Headers["Authorization"].First().Split(" ").Last();

            return String.Empty;
        }

        private JwtSecurityToken? GetToken(string headerValue, string secret)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.ASCII.GetBytes(secret);

            try
            {
                tokenHandler.ValidateToken(headerValue, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return (JwtSecurityToken)validatedToken; ;
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }
    }
}
