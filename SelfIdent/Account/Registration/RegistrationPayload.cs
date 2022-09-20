using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.Registration;

public class RegistrationPayload : Payload, IEndpointPayload
{
    public RegistrationPayload(HttpContext context, string username, string email, string password) : base(context)
    {
        this.Username = username;
        this.Email = email;
        this.Password = password;

        this.Roles = null;
    }

    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    /// <summary>
    /// Roles to set for the new User. Sets the Default-Roles if Null or Empty.
    /// Sets the Default-Roles if the roles are not defined in the Setup.
    /// </summary>
    public List<Roles.Role>? Roles { get; set; }

}
