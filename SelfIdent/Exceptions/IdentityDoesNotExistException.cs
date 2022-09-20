using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Exceptions;

public class IdentityDoesNotExistException : Exception
{
    private static string _baseMessage = "No User with this email or name was found.";

    public IdentityDoesNotExistException(string message) : base(_baseMessage + message)
    {

    }

    public IdentityDoesNotExistException() : base(String.Format(_baseMessage))
    {

    }
}
