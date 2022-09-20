using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Enums;

/// <summary>
/// When the SecurityContext should be invalidated based on IdentityChanges
/// -
/// Never                : SecurityContext never gets invalidated based on Updates
/// KeyInformationChange : SecurityContext gets invalidated when Key information changes. Like Password, Email or Username
/// PasswordChange       : SecurityContext gets invalidated when the Password is changed
/// Always               : SecurityContext gets invalidated on every Identity update
/// </summary>
public enum SecurityContextInvalidationStrictness
{
    Never = 0,
    PasswordChange = 1,
    KeyInformationChange = 2,
    Always = 3
}
