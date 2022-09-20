using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Token;

public class AuthenticationToken
{
    /// <summary>
    /// The Actual String that gets authenticated against
    /// </summary>
    public string? AccessToken { get; set; }
    /// <summary>
    /// When the Token loses validity
    /// </summary>
    public DateTime? ExpiresAt { get; set; }
}
