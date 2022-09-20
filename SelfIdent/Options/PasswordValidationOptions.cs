using System;
using System.Collections.Generic;
using System.Text;

namespace SelfIdent.Options;

public class PasswordValidationOptions
{
    public enum PasswordValidationPresets
    {
        None,
        Unsafe,
        Medium,
        Safe
    }

    public int MinLength { get; set; }
    public int MaxLength { get; set; }
    public bool RequireUppercase { get; set; }
    public bool RequireNumber { get; set; }
    public bool RequireSpecialSymbol { get; set; }
    /// <summary>
    /// If the "CustomValidationFunction" should be used
    /// </summary>
    public bool UseCustomValidationFunction { get; set; }
    /// <summary>
    /// A Custom Function that gets called for additional validation if "UseCustomValidationFunction" is true
    /// </summary>
    public Func<bool>? CustomValidationFunction { get; set; }

    public PasswordValidationOptions(int minLength, int maxLength, bool requireUppercase = true, bool requireNumber = true, bool requireSpecialSymbol = true, bool useCustomValidationFunction = false, Func<bool>? customValidationFunction = null)
    {
        MinLength = minLength;
        MaxLength = maxLength;
        RequireUppercase = requireUppercase;
        RequireNumber = requireNumber;
        RequireSpecialSymbol = requireSpecialSymbol;
        UseCustomValidationFunction = useCustomValidationFunction;
        CustomValidationFunction = customValidationFunction;
    }

    /// <summary>
    /// Sets the Validationoptions to a preset. "Safe" is recommended
    /// </summary>
    /// <param name="preset"></param>
    public PasswordValidationOptions(PasswordValidationPresets preset)
    {
        switch (preset)
        {
            case PasswordValidationPresets.Unsafe:
                MinLength = 5;
                MaxLength = 40;
                RequireUppercase = false;
                RequireNumber = false;
                RequireSpecialSymbol = false;
                UseCustomValidationFunction = false;
                CustomValidationFunction = null;
                break;
            case PasswordValidationPresets.Medium:
                MinLength = 8;
                MaxLength = 40;
                RequireUppercase = false;
                RequireNumber = true;
                RequireSpecialSymbol = false;
                UseCustomValidationFunction = false;
                CustomValidationFunction = null;
                break;
            case PasswordValidationPresets.Safe:
                MinLength = 10;
                MaxLength = 40;
                RequireUppercase = true;
                RequireNumber = true;
                RequireSpecialSymbol = true;
                UseCustomValidationFunction = false;
                CustomValidationFunction = null;
                break;
            case PasswordValidationPresets.None:
                MinLength = 1;
                MaxLength = 40;
                RequireUppercase = false;
                RequireNumber = false;
                RequireSpecialSymbol = false;
                UseCustomValidationFunction = false;
                CustomValidationFunction = null;
                break;
            default:
                break;
        }
    }
}
