using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Exceptions;

/// <summary>
/// This Exception gets thrown when a critical state issue occurs.
/// For example if the Data in the Database is not as expected and has likely been tampered with.
/// </summary>
public class CriticalStateException : Exception
{
    private static string _baseMessage = "A critical, unexpected error occured. This might be because the state of the Database is invalid. ";

    public CriticalStateException(string message) : base(_baseMessage + message)
    {

    }

    public CriticalStateException() : base(String.Format(_baseMessage))
    {

    }
}
