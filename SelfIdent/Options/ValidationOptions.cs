using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Interfaces;

namespace SelfIdent.Options;

/// <summary>
/// Options for Validation at Registration.
/// 
/// These options implement 'Security by Default'.
/// The Default Options are purposefully strict to avoid security-issues if an options is not considered.
/// </summary>
public class ValidationOptions
{
    public bool ValidateEmailOnRegister { get; set; } = true;
    public bool ValidatePasswordOnRegister { get; set; } = true;
    public bool ValidateUsernameOnRegister { get; set; } = true;
    /// <summary>
    /// Options for PasswordValidation 
    /// </summary>
    public PasswordValidationOptions PasswordValidationOptions { get; set; } = new PasswordValidationOptions(PasswordValidationOptions.PasswordValidationPresets.Safe);
    /// <summary>
    /// Options for EmailValidation 
    /// </summary>
    public EmailValidationOptions EmailValidationOptions { get; set; } = new EmailValidationOptions(EmailValidationOptions.EmailValidationPresets.Standard);
    /// <summary>
    /// Options for UsernameValidation 
    /// </summary>
    public UsernameValidationOptions UsernameValidationOptions { get; set; } = new UsernameValidationOptions();
    /// <summary>
    /// Custom Validator for Registration
    /// </summary>
    public IValidator? CustomValidator { get; set; }
}
