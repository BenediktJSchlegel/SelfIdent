using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Interfaces;

namespace SelfIdent.Options.Hashing;

public class ArgonHashingOptions : IHashingOptions
{
    public int SaltByteLength { get; set; } = 128 / 8; // 128 Bit | 16 Byte
    public int HashByteLength { get; set; } = 256 / 8; // 256 Bit | 32 Byte
    public int Iterations { get; set; } = 10000;

    /// <summary>
    /// The Amount of Threads initiated by each call to Argon2
    /// </summary>
    public int Threads { get; set; } = 2;
    /// <summary>
    /// The Amount of Memory each call may use in kB
    /// </summary>
    public int MemoryInKB { get; set; } = 1024;
    /// <summary>
    /// The amount of Time each call may take (In Seconds)
    /// </summary>
    public int TimeInSeconds { get; set; } = 1;

    public bool IsEqual(IHashingOptions instance)
    {
        if (instance == null || !(instance is ArgonHashingOptions))
            return false;

        var convertedInstance = (ArgonHashingOptions)instance;

        return this.SaltByteLength == convertedInstance.SaltByteLength
            && this.HashByteLength == convertedInstance.HashByteLength
            && this.Iterations == convertedInstance.Iterations;
    }
}
