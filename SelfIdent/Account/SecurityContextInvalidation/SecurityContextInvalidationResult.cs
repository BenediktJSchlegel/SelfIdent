using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.SecurityContextInvalidation;

public class SecurityContextInvalidationResult : IEndpointResult
{
    public SecurityContextInvalidationResult(SecurityContextInvalidationPayload payload)
    {
        this.Payload = payload;
    }

    public SecurityContextInvalidationPayload Payload { get; set; }
    public bool Successful { get; set; } = true;
    public Exception? ThrownException { get; set; }
    public User? InvalidatedUser { get; set; }
}
