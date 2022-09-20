using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SelfIdent.Account.SecurityContext;

internal class CookieContextPayload : ContextPayload
{
    public CookieContextPayload(HttpContext context, User user) : base(context, user)
    {
        //
    }
    public bool PersistentCookie { get; set; } = false;
    public string? RedirectUri { get; set; }
    public bool? AllowRefresh { get; set; }
    public DateTimeOffset? ExpiresUtc { get; set; }

}
