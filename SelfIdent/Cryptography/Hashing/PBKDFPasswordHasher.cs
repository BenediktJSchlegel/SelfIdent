using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SelfIdent.Interfaces;
using SelfIdent.Options.Hashing;

namespace SelfIdent.Cryptography.Hashing;

internal class PBKDFPasswordHasher : IPasswordHasher
{
    private PBKDFHashingOptions _options;

    public PBKDFPasswordHasher(PBKDFHashingOptions options)
    {
        _options = options;
    }

    public byte[] HashPassword(string password, byte[] salt, char pepper)
    {
        return HashPassword(password, salt, pepper, _options);
    }

    public byte[] HashPassword(string password, byte[] salt, char pepper, IHashingOptions options)
    {
        if (options == null || !(options is PBKDFHashingOptions))
            throw new ArgumentException($"Invalid Options passed to {nameof(PBKDFPasswordHasher)}");

        return KeyDerivation.Pbkdf2(password + pepper, salt, ((PBKDFHashingOptions)options).HashingFunction, options.Iterations, options.HashByteLength);
    }
}
