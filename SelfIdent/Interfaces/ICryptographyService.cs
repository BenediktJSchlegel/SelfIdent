using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Cryptography;
using SelfIdent.Options;

namespace SelfIdent.Interfaces;

internal interface ICryptographyService
{
    PasswordHashResult HashPassword(string password);
    bool MatchPasswords(string password, string salt, string hash, IHashingOptions options);
    string GenerateSemiRandomKey(int length);
}
