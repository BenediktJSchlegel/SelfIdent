using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Exceptions;

public class UserOrEmailAlreadyInUseException : Exception
{
    private static string _baseMessage = "Username or Email is already in use.";

    public UserOrEmailAlreadyInUseException(string message) : base(_baseMessage + message)
    {

    }

    public UserOrEmailAlreadyInUseException() : base(String.Format(_baseMessage))
    {

    }
}
