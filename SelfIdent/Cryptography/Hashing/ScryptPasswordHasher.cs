using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Scrypt;
using SelfIdent.Interfaces;
using SelfIdent.Options.Hashing;

namespace SelfIdent.Cryptography.Hashing;

internal class ScryptPasswordHasher : IPasswordHasher
{
    private ScryptHashingOptions _options;

    public ScryptPasswordHasher(ScryptHashingOptions options)
    {
        _options = options;
    }

    public byte[] HashPassword(string password, byte[] salt, char pepper)
    {
        return HashPassword(password, salt, pepper, _options);
    }

    public byte[] HashPassword(string password, byte[] salt, char pepper, IHashingOptions options)
    {
        if (options == null || !(options is ScryptHashingOptions))
            throw new ArgumentException($"Invalid Options passed to {nameof(ScryptPasswordHasher)}");

        var scryptOptions = (ScryptHashingOptions)options;

        var encoder = new ScryptEncoder(scryptOptions.Iterations, scryptOptions.BlockSize, scryptOptions.Threads);

        return Convert.FromBase64String(encoder.Encode(password + Convert.ToBase64String(salt) + pepper));
    }
}
