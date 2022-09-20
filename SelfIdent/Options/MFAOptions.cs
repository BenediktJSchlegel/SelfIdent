using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Options;

public class MFAOptions
{
    /// <summary>
    /// If the Session-Key needs to match between Authentication and MFA Requests
    /// </summary>
    public bool CheckSessionKey { get; set; } = true;
    /// <summary>
    /// Length of the generated Key
    /// </summary>
    public int KeyLength { get; set; } = 8;
    /// <summary>
    /// How long the Key is valid for. After this Credentials have to be given again to generate a new Key.
    /// Defaults to 5 minutes.
    /// </summary>
    public TimeSpan KeyTimeout { get; set; } = TimeSpan.FromMinutes(5);
}
