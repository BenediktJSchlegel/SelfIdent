using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SelfIdent.Account.Authentication;

public class CookieAuthenticationPayload : AuthenticationPayload
{
    /// <summary>
    /// If the Cookie should persist across sessions
    /// </summary>
    public bool PersistentCookie { get; set; } = false;
    public string? RedirectUri { get; set; }
    public bool? AllowRefresh { get; set; }
    public DateTimeOffset? ExpiresUtc { get; set; }

    public CookieAuthenticationPayload(HttpContext context, string username, string email, string password) : base(context, username, email, password)
    {
        //
    }
}
