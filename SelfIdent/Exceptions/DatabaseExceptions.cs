using System;

namespace SelfIdent.Exceptions;

public class DatabaseConnectionFailedException : Exception
{
    private static string _baseMessage = "Connection could not be made! ConnectionString might be invalid. (ConnectionString: {0} )";

    public DatabaseConnectionFailedException(string connectionString, string message) : base(String.Format(_baseMessage, connectionString) + message)
    {
        
    }

    public DatabaseConnectionFailedException(string connectionString) : base(String.Format(_baseMessage, connectionString))
    {

    }
}
