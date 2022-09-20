using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SelfIdent.Interfaces;

namespace SelfIdent.Options.Hashing;

public class PBKDFHashingOptions : IHashingOptions
{
    /// <summary>
    /// Total Byte Length of the Salt
    /// </summary>
    public int SaltByteLength { get; set; } = 128 / 8; // 128 Bit | 16 Byte
    /// <summary>
    /// Total Byte Length of the Hash
    /// </summary>
    public int HashByteLength { get; set; } = 256 / 8; // 256 Bit | 32 Byte
    /// <summary>
    /// The Amount of Iterations the Hashing Function should use.
    /// If there are already Hashed Passwords in the Database with less iterations, the Password will be rehashed 
    /// and updated on next Authentication of that user.
    /// </summary>
    public int Iterations { get; set; } = 10000;
    /// <summary>
    /// The hashfunction to be used for password hashing
    /// </summary>
    public KeyDerivationPrf HashingFunction { get; set; } = KeyDerivationPrf.HMACSHA256;

    /// <summary>
    /// Checks if the Options of the given instance are equal to this instance.
    /// </summary>
    /// <param name="instance"></param>
    /// <returns></returns>
    public bool IsEqual(IHashingOptions instance)
    {
        if (instance == null || !(instance is PBKDFHashingOptions))
            return false;

        var convertedInstance = (PBKDFHashingOptions)instance;

        return this.SaltByteLength == convertedInstance.SaltByteLength
            && this.HashByteLength == convertedInstance.HashByteLength
            && this.Iterations == convertedInstance.Iterations
            && this.HashingFunction == convertedInstance.HashingFunction;
    }
}
