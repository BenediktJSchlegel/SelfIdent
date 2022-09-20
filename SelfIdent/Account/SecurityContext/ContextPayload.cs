using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SelfIdent.Account.Authentication;

namespace SelfIdent.Account.SecurityContext;

public class ContextPayload
{
    public HttpContext Context { get; set; }
    public User User { get; set; }

    public ContextPayload(HttpContext context, User user)
    {
        this.Context = context;
        this.User = user;
    }
}
