using System;
using System.Collections.Generic;
using System.Text;

namespace SelfIdent.Validation;

public class ValidationResult
{
    public PasswordValidationResult? Password { get; set; }
    public UsernameValidationResult? Username { get; set; }
    public EmailValidationResult? Email { get; set; }

    public bool IsInvalid
    {
        get
        {
            return (Password == null || Password.IsInvalid) ||
                   (Username == null || Username.IsInvalid) ||
                   (Email == null || Email.IsInvalid);
        }
    }
}

public class AlwaysValidPasswordValidationResult : PasswordValidationResult
{
    public new bool IsInvalid => true;
}

public class AlwaysValidEmailValidationResult : EmailValidationResult
{
    public new bool IsInvalid => true;
}

public class AlwaysValidUsernameValidationResult : UsernameValidationResult
{
    public new bool IsInvalid => true;
}

public class PasswordValidationResult
{
    public bool MinLengthOK { get; set; } = true;
    public bool MaxLengthOK { get; set; } = true;
    public bool UppercaseOK { get; set; } = true;
    public bool NumberOK { get; set; } = true;
    public bool SpecialSymbolOK { get; set; } = true;
    public bool CustomFunctionOK { get; set; } = true;

    public bool IsInvalid
    {
        get
        {
            return !MinLengthOK || !MaxLengthOK || !UppercaseOK || !NumberOK || !SpecialSymbolOK || !CustomFunctionOK;
        }
    }

}

public class EmailValidationResult
{
    public bool AtOK { get; set; } = true;
    public bool CustomFunctionOK { get; set; } = true;
    public bool FullCheckOK { get; set; } = true;

    public bool IsInvalid
    {
        get
        {
            return !AtOK || !CustomFunctionOK || !FullCheckOK;
        }
    }

}

public class UsernameValidationResult
{
    public bool LengthOk { get; set; } = true;
    public bool CustomFunctionOk { get; set; } = true;

    public bool IsInvalid
    {
        get
        {
            return !LengthOk || !CustomFunctionOk;
        }
    }
}
