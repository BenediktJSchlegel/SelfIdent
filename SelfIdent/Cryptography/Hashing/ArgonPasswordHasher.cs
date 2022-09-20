using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Konscious.Security.Cryptography;
using SelfIdent.Interfaces;
using SelfIdent.Options.Hashing;

namespace SelfIdent.Cryptography.Hashing;

internal class ArgonPasswordHasher : IPasswordHasher
{
    private ArgonHashingOptions _options;

    public ArgonPasswordHasher(ArgonHashingOptions options)
    {
        _options = options;
    }

    public byte[] HashPassword(string password, byte[] salt, char pepper)
    {
        return HashPassword(password, salt, pepper, _options);
    }

    public byte[] HashPassword(string password, byte[] salt, char pepper, IHashingOptions options)
    {
        if (options == null || !(options is ArgonHashingOptions))
            throw new ArgumentException($"Invalid Options passed to {nameof(ArgonPasswordHasher)}");

        var argonOptions = (ArgonHashingOptions)options;

        var argon = new Argon2id(Encoding.UTF8.GetBytes(password + pepper));

        argon.Salt = salt;
        argon.DegreeOfParallelism = argonOptions.Threads;
        argon.Iterations = argonOptions.Iterations;
        argon.MemorySize = argonOptions.MemoryInKB;

        return argon.GetBytes(argonOptions.HashByteLength);
    }
}
