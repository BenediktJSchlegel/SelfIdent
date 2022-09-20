using SelfIdent.Account.Registration;
using System;

namespace SelfIdent.Exceptions;

public class ValidationFailedException : Exception
{
    private static string _baseMessage = "Validation of RegistrationPayload failed!";

    public ValidationFailedException(RegistrationPayload payload, string message) : base(_baseMessage + message)
    {

    }

    public ValidationFailedException(RegistrationPayload payload) : base(_baseMessage)
    {

    }
}
