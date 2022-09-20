using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.Lock;

public class UnlockResult : IEndpointResult
{
    public UnlockResult(UnlockPayload payload)
    {
        Payload = payload;
    }

    public UnlockPayload Payload { get; set; }
    public User? User { get; set; }
    public bool Successful { get; set; } = true;
    public Exception? ThrownException { get; set; }
}
