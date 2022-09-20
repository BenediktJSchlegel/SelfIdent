using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.Update;

public class UpdateResult : IEndpointResult
{
    public UpdateResult(UpdatePayload payload)
    {
        Payload = payload;
    }

    public UpdatePayload Payload { get; set; }
    public User? User { get; set; }
    public bool? Successful { get; set; } = true;
    public Exception? ThrownException { get; set; }
}
