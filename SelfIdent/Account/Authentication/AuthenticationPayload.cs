using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Http;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.Authentication;

/// <summary>
/// Payload sent to NetIdentity to Authenticate
/// </summary>
public class AuthenticationPayload : Payload, IEndpointPayload
{
    public AuthenticationPayload(HttpContext context, string username, string email, string password) : base(context)
    {
        this.Username = username;
        this.Email = email;
        this.Password = password;
    }

    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
