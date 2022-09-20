using System;
using System.Collections.Generic;
using System.Runtime;
using System.Text;
using System.Text.RegularExpressions;
using SelfIdent.Interfaces;
using SelfIdent.Options;
using SelfIdent.Account.Registration;

namespace SelfIdent.Validation;

internal class Validator : IValidator
{
    private ValidationOptions _options;

    private string _upperCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    private string _numbers = "0123456789";
    private string _specialCharacters = "! \"#$%&'()*+,-./:;<=>?@[\\]^_`{|}~";

    public Validator(ValidationOptions options)
    {
        _options = options;
    }

    public ValidationResult Validate(RegistrationPayload payload)
    {
        if (payload == null)
            throw new ArgumentException(nameof(payload));

        var result = new ValidationResult();

        if (_options.ValidateEmailOnRegister)
            result.Email = CheckEmailValidity(payload.Email);
        else
            result.Email = new AlwaysValidEmailValidationResult();

        if (_options.ValidatePasswordOnRegister)
            result.Password = CheckPasswordValidity(payload.Password);
        else
            result.Password = new AlwaysValidPasswordValidationResult();

        if (_options.ValidateUsernameOnRegister)
            result.Username = CheckUsernameValidity(payload.Username);
        else
            result.Username = new AlwaysValidUsernameValidationResult();

        return result;
    }

    private UsernameValidationResult CheckUsernameValidity(string username)
    {
        var result = new UsernameValidationResult();

        UsernameValidationOptions options = _options.UsernameValidationOptions;

        if (String.IsNullOrEmpty(username) || username.Length > options.MaxLength || username.Length < options.MinLength)
            result.LengthOk = false;

        if (options.UseCustomValidationFunction)
        {
            if (options.CustomValidationFunction == null)
                result.CustomFunctionOk = false;
            else
                result.CustomFunctionOk = options.CustomValidationFunction();
        }

        return result;
    }

    private PasswordValidationResult CheckPasswordValidity(string password)
    {
        var result = new PasswordValidationResult();

        PasswordValidationOptions options = _options.PasswordValidationOptions;

        if (password.Length < options.MinLength)
            result.MinLengthOK = false;

        if (password.Length > options.MaxLength)
            result.MaxLengthOK = false;

        if (options.RequireNumber && !StringContainsChar(password, _numbers.ToCharArray()))
            result.NumberOK = false;

        if (options.RequireUppercase && !StringContainsChar(password, _upperCharacters.ToCharArray()))
            result.UppercaseOK = false;

        if (options.RequireSpecialSymbol && !StringContainsChar(password, _specialCharacters.ToCharArray()))
            result.SpecialSymbolOK = false;

        if (options.UseCustomValidationFunction)
        {
            if (options.CustomValidationFunction == null)
                result.CustomFunctionOK = false;
            else
                result.CustomFunctionOK = options.CustomValidationFunction();
        }

        return result;
    }

    private EmailValidationResult CheckEmailValidity(string email)
    {
        var result = new EmailValidationResult();

        EmailValidationOptions options = _options.EmailValidationOptions;

        if (options.CheckForAt && !StringContainsChar(email, new char[] { '@' }))
            result.AtOK = false;

        if (options.CheckFullValidity && !EmailFullyCheckIsValid(email))
            result.FullCheckOK = false;

        if (options.UseCustomValidationFunction)
        {
            if (options.CustomValidationFunction == null)
                result.CustomFunctionOK = false;
            else
                result.CustomFunctionOK = options.CustomValidationFunction();
        }

        return result;
    }

    private bool EmailFullyCheckIsValid(string email)
    {
        string validEmailPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                        + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                        + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";

        return new Regex(validEmailPattern, RegexOptions.IgnoreCase).IsMatch(email);
    }

    private bool StringContainsChar(string str, char[] chars)
    {
        foreach (char ch in chars)
        {
            if (str.Contains(ch))
                return true;
        }

        return false;
    }
}


