using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Options;

public class SecurityContextOptions
{
    public enum SecurityContextAuthenticationTypes
    {
        /// <summary>
        /// No persistent Authentication
        /// </summary>
        None,
        /// <summary>
        /// Cookie Authentication
        /// </summary>
        Cookies,
        /// <summary>
        /// Token Authentication
        /// </summary>
        Token,
        /// <summary>
        /// Cookie and Token Authentication
        /// </summary>
        CookieAndToken
    }

    /// <summary>
    /// What Type of persistent Authentication to perform
    /// </summary>
    public SecurityContextAuthenticationTypes Type { get; set; }
    /// <summary>
    /// The Name of the AuthenticationSchema. This is Key for the .Net Middleware to map Cookies or Tokens to the SecurityContext
    /// </summary>
    public string AuthenticationSchema { get; set; }
    /// <summary>
    /// Path to when the User should Log in to access a Site
    /// </summary>
    public string? LoginPath { get; set; }
    /// <summary>
    /// Path to navigate to when logging out
    /// </summary>
    public string? LogoutPath { get; set; }
    /// <summary>
    /// The Key used to Hash the AuthenticationToken
    /// </summary>
    public string? TokenSecretKey { get; set; }
    /// <summary>
    /// How long generated Tokens stay valid
    /// </summary>
    public TimeSpan TokenLifetime { get; set; }

    public SecurityContextOptions(string authenticationSchema, SecurityContextAuthenticationTypes type)
    {
        this.AuthenticationSchema = authenticationSchema;
        this.Type = type;
    }
}
