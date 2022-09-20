using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Exceptions;

public class InvalidOptionException : Exception
{
    private static string _baseMessage = "Given Options were not valid.";

    public InvalidOptionException(string message) : base(_baseMessage + message)
    {

    }

    public InvalidOptionException() : base(String.Format(_baseMessage))
    {

    }
}
