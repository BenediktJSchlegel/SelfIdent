using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Interfaces;

namespace SelfIdent.Options.Hashing;

public class ScryptHashingOptions : IHashingOptions
{
    public int SaltByteLength { get; set; } = 128 / 8; // 128 Bit | 16 Byte
    public int HashByteLength { get; set; } = 256 / 8; // 256 Bit | 32 Byte
    public int Iterations { get; set; } = 10000;
    public int Threads { get; set; } = 2;
    public int BlockSize { get; set; } = 1024;

    public bool IsEqual(IHashingOptions instance)
    {
        if (instance == null || !(instance is ScryptHashingOptions))
            return false;

        var convertedInstance = (ScryptHashingOptions)instance;

        return this.SaltByteLength == convertedInstance.SaltByteLength
            && this.HashByteLength == convertedInstance.HashByteLength
            && this.Iterations == convertedInstance.Iterations;
    }
}
