using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.Deletion;

public class DeletionResult : IEndpointResult
{
    public bool Successful { get; set; }
    public User? User { get; set; }
    public Exception? ThrownException { get; set; }
}
