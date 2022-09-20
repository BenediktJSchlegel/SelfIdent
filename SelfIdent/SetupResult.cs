using System;

namespace SelfIdent;

public class SetupResult
{
    /// <summary>
    /// Was Successful => Can continue
    /// </summary>
    public bool Successful { get; set; }
    /// <summary>
    /// There were Tables in the given DB usable by NetIdentity
    /// </summary>
    public bool FoundExistingTables { get; set; }
    /// <summary>
    /// Tables were Created to be used by NetIdent
    /// </summary>
    public bool CreatedTables { get; set; }
    /// <summary>
    /// Connection Failed!
    /// </summary>
    public bool ConnectionFailed { get; set; }
    /// <summary>
    /// The Exception that was thrown if Successful = false
    /// </summary>
    public Exception? ThrownException { get; set; }
}