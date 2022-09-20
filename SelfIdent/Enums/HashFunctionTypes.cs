using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Enums;

public enum HashFunctionTypes
{
    Unknown = 0,
    Argon = 1,
    PBKDF = 2,
    Scrypt = 3
}
