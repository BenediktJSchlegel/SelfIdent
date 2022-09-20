using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Exceptions;

public class SetupNotCompleteException : Exception
{
    private static string _baseMessage = "Setup was never completed. ConnectionString or Connection were not available!";

    public SetupNotCompleteException(string message) : base(_baseMessage + message)
    {

    }

    public SetupNotCompleteException() : base(_baseMessage)
    {

    }
}
