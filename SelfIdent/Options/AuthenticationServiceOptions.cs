using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Options;

public class AuthenticationServiceOptions
{
    public enum SecurityContextTypes
    {
        Cookie,
        Token
    }

    public SecurityContextTypes SecurityContextType { get; set; }
    public string SchemaName { get; set; }
    public string? LoginPath { get; set; }
    public string? LogoutPath { get; set; }
    public AuthenticationValidationOptions? AuthenticationValidationOptions { get; set; }

    //TODO: Fill rest of Settings

    public AuthenticationServiceOptions(SecurityContextTypes type, string schema)
    {
        this.SecurityContextType = type;
        this.SchemaName = schema;
    }
}

