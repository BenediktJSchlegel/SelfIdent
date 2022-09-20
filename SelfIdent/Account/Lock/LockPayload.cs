using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.Lock;

public class LockPayload : Payload, IEndpointPayload
{
    public LockPayload(HttpContext context, ulong id) : base(context)
    {
        this.Id = id;
    }

    public ulong Id { get; set; }
}
