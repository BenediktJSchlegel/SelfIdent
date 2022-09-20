using System;
using System.Collections.Generic;
using System.Text;
using SelfIdent.Interfaces;
using SelfIdent.Options;

namespace SelfIdent.Cryptography;

internal class PasswordHashResult
{
    /// <summary>
    /// Bytes representing the Hashed Password. Including Password, Salt & Pepper
    /// </summary>
    public byte[]? HashBytes { get; set; }
    /// <summary>
    /// Bytes representing the Salt
    /// </summary>
    public byte[]? SaltBytes { get; set; }
    /// <summary>
    /// Hashbytes in Base64String Format
    /// </summary>
    public string Hash => HashBytes != null ? Convert.ToBase64String(HashBytes) : String.Empty;
    /// <summary>
    /// The Salt Appended to the Password before Hashing
    /// </summary>
    public string Salt => SaltBytes != null ? Convert.ToBase64String(SaltBytes) : String.Empty;
    /// <summary>
    /// Options used for Hashing
    /// </summary>
    public IHashingOptions HashingOptions { get; set; }

    public PasswordHashResult(IHashingOptions options)
    {
        this.HashingOptions = options;
    }
}
