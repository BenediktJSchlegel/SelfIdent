using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using SelfIdent.Interfaces;
using SelfIdent.Options;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SelfIdent.Cryptography;

internal class CryptographyService : ICryptographyService
{
    // https://github.com/dotnet/aspnetcore/blob/891dd0fa6f16c4cd24aaeead57ece03fb098d83a/src/Identity/Extensions.Core/src/PasswordHasher.cs#L256
    /// <summary>
    /// Use .Net Hashing Alg.
    /// Actual Hashing, Salting etc Format should be own.
    /// Pepper not included in .Net Example?
    /// </summary>

    private string _pepperString = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    private IHashingOptions _options;
    private IPasswordHasher _hasher;

    public CryptographyService(IHashingOptions options, IPasswordHasher hasher)
    {
        _options = options;
        _hasher = hasher;
    }

    public PasswordHashResult HashPassword(string password)
    {
        var result = new PasswordHashResult(_options);

        byte[] salt = GenerateSalt(_options.SaltByteLength);
        char pepper = GeneratePepper();

        byte[] hash = _hasher.HashPassword(password, salt, pepper);

        result.HashBytes = hash;
        result.SaltBytes = salt;

        return result;
    }

    public bool MatchPasswords(string password, string salt, string hash, IHashingOptions options)
    {
        char[] peppers = _pepperString.ToCharArray();

        foreach (char pepper in peppers)
        {
            byte[] newHashBytes = _hasher.HashPassword(password, Convert.FromBase64String(salt), pepper, options);
            byte[] hashBytes = Convert.FromBase64String(hash);

            if (ByteArraysAreEqual(newHashBytes, hashBytes))
                return true;
        }

        return false;
    }

    private bool ByteArraysAreEqual(byte[] c1, byte[] c2)
    {
        if (c1 == null || c2 == null)
        {
            // Can not compare if nothing is there
            return false;
        }
        else
        {
            if (c1.Length != c2.Length)
                return false;
        }

        bool areEqual = true;

        for (int i = 0; i < c1.Length; i++)
            areEqual &= (c1[i] == c2[i]);

        return areEqual;
    }

    private byte[] GenerateSalt(int length)
    {
        byte[] bytes = new byte[length];

        using (var generator = RandomNumberGenerator.Create())
            generator.GetBytes(bytes);

        return bytes;
    }


    private char GeneratePepper()
    {
        var rnd = new Random();
        char[] chars = _pepperString.ToCharArray();

        return chars[rnd.Next(chars.Length)];
    }

    public string GenerateSemiRandomKey(int length)
    {
        byte[] bytes = new byte[length];

        using (var generator = RandomNumberGenerator.Create())
        {
            generator.GetBytes(bytes);
            uint bytesAsNumber = BitConverter.ToUInt32(bytes, 0);

            return bytesAsNumber.ToString();
        }
    }
}

