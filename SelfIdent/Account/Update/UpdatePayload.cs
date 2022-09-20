using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SelfIdent.Interfaces;

namespace SelfIdent.Account.Update;

public class UpdatePayload : Payload, IEndpointPayload
{
    public UpdatePayload(HttpContext context, ulong id) : base(context)
    {
        this.Id = id;
    }

    /// <summary>
    /// The ID by which the User will be updated.
    /// </summary>
    public ulong Id { get; set; }
    /// <summary>
    /// Username. If String.Empty the Username will not be updated.
    /// </summary>
    public string? Username { get; set; }
    /// <summary>
    /// Password. If String.Empty the Password will not be updated.
    /// </summary>
    public string? Password { get; set; }
    /// <summary>
    /// Email. If String.Empty the Email will not be updated.
    /// </summary>
    public string? Email { get; set; }
    /// <summary>
    /// Roles of the User. Does not get updated if null
    /// </summary>
    public List<Roles.Role>? Roles { get; set; }
}
