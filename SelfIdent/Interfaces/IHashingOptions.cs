using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Interfaces;

public interface IHashingOptions
{
    /// <summary>
    /// Total Byte Length of the Salt
    /// </summary>
    int SaltByteLength { get; set; }
    /// <summary>
    /// Total Byte Length of the Hash
    /// </summary>
    int HashByteLength { get; set; }
    /// <summary>
    /// The Amount of Iterations the Hashing Function should use.
    /// If there are already Hashed Passwords in the Database with less iterations, the Password will be rehashed 
    /// and updated on next Authentication of that user.
    /// </summary>
    int Iterations { get; set; }

    bool IsEqual(IHashingOptions instance);

}
