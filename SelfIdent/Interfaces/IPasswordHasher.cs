using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Interfaces;

internal interface IPasswordHasher
{
    /// <summary>
    /// Hash the given Password using the given salt and pepper.
    /// Uses Default Options passed through the constructor
    /// </summary>
    /// <param name="password"></param>
    /// <param name="salt"></param>
    /// <param name="pepper"></param>
    /// <returns></returns>
    byte[] HashPassword(string password, byte[] salt, char pepper);
    /// <summary>
    /// Hash the given Password using the given salt and pepper.
    /// Uses the given Options.
    /// </summary>
    /// <param name="password"></param>
    /// <param name="salt"></param>
    /// <param name="pepper"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    byte[] HashPassword(string password, byte[] salt, char pepper, IHashingOptions options);
}
