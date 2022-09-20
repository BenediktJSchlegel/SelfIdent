using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Options;

public class UsernameValidationOptions
{
    /// <summary>
    /// If the Username has to be unique in the Database
    /// </summary>
    public bool MustBeUnique { get; set; } = true;
    /// <summary>
    /// Max Characters
    /// </summary>
    public int MaxLength { get; set; } = Int32.MaxValue;
    /// <summary>
    /// Min Characters
    /// </summary>
    public int MinLength { get; set; } = 0;
    /// <summary>
    /// If the "CustomValidationFunction" should be used
    /// </summary>
    public bool UseCustomValidationFunction { get; set; }
    /// <summary>
    /// A Custom Function that gets called for additional validation if "UseCustomValidationFunction" is true
    /// </summary>
    public Func<bool>? CustomValidationFunction { get; set; }
}
