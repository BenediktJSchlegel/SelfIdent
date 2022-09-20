using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.Lock;

public class UnlockPayload : Payload, IEndpointPayload
{
    public UnlockPayload(HttpContext context, ulong id, string key) : base(context)
    {
        this.Id = id;
        this.Key = key;
    }

    public ulong Id { get; set; }
    public string Key { get; set; }
}
