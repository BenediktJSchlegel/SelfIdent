using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.Registration;

public class RegistrationResult : IEndpointResult
{
    public RegistrationResult(RegistrationPayload payload)
    {
        Payload = payload;
    }

    public RegistrationPayload Payload { get; set; }
    public bool Successful { get; set; } = true;
    public User? GeneratedUser { get; set; }
    public Exception? ThrownException { get; set; }
}
