using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace SelfIdent.Account.SecurityContextInvalidation;

public class SecurityContextInvalidationPayload : Payload
{
    public SecurityContextInvalidationPayload(HttpContext context, ulong userId) : base(context)
    {
        this.Id = userId;
    }

    public ulong Id { get; set; }

}
