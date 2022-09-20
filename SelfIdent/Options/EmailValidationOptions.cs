using System;
using System.Collections.Generic;
using System.Text;

namespace SelfIdent.Options;

public class EmailValidationOptions
{
    public enum EmailValidationPresets
    {
        None,
        Standard
    }

    public bool CheckForAt { get; set; }
    public bool CheckFullValidity { get; set; }
    /// <summary>
    /// If the "CustomValidationFunction" should be used
    /// </summary>
    public bool UseCustomValidationFunction { get; set; }
    /// <summary>
    /// A Custom Function that gets called for additional validation if "UseCustomValidationFunction" is true
    /// </summary>
    public Func<bool>? CustomValidationFunction { get; set; }

    public EmailValidationOptions(bool checkForAt, bool checkFullValidity, bool useCustomValidationFunction = false, Func<bool>? customValidationFunction = null)
    {
        CheckForAt = checkForAt;
        CheckFullValidity = checkFullValidity;
        UseCustomValidationFunction = useCustomValidationFunction;
        CustomValidationFunction = customValidationFunction;
    }

    public EmailValidationOptions(EmailValidationPresets preset)
    {
        switch (preset)
        {
            case EmailValidationPresets.None:
                CheckForAt = false;
                CheckFullValidity = false;
                UseCustomValidationFunction = false;
                CustomValidationFunction = null;
                break;
            case EmailValidationPresets.Standard:
                CheckForAt = true;
                CheckFullValidity = true;
                UseCustomValidationFunction = false;
                CustomValidationFunction = null;
                break;
            default:
                break;
        }
    }
}
