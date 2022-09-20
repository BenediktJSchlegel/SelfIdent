using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Identity;

internal class IdentityAccountStatus
{
    /// <summary>
    /// Locked out for some reason (Banned/Too many failed attempts etc)
    /// </summary>
    public bool Locked { get; set; } = false;
    /// <summary>
    /// Key to unlock a locked Account
    /// </summary>
    public string? LockKey { get; set; }
    /// <summary>
    /// Counter for amount of login tries
    /// </summary>
    public int? LoginAttempts { get; set; }
    /// <summary>
    /// Date and Time of registration
    /// </summary>
    public DateTime? RegistrationTime { get; set; }
    /// <summary>
    /// Date and Time of last login
    /// </summary>
    public DateTime? LastLogin { get; set; }
    
}
