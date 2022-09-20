using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.Lock;

public class LockResult : IEndpointResult
{
    public LockResult(LockPayload payload)
    {
        Payload = payload;
    }

    public LockPayload Payload { get; set; }
    public User? User { get; set; }
    public string? LockKey { get; set; }
    public bool Successful { get; set; } = true;
    public Exception? ThrownException { get; set; }
}
