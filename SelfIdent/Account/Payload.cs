using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SelfIdent.Account;

public abstract class Payload
{
    public HttpContext Context { get; set; }

    public Payload(HttpContext context)
    {
        this.Context = context;
    }
}
