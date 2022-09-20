using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Enums;

/// <summary>
/// Levels of Integrity of a SecurityContext associated with an Identity
/// -
/// VALID   : Nothing has Changed
/// UPDATE  : The Claims need to be Updated, but the SecurityContext is still valid
/// INVALID : The Identity has changed in a meaningful way, or the SecurityContext was manually invalidated. Reauthentication is required.
/// </summary>
internal enum SecurityContextIntegrityStatus
{
    VALID = 0,
    UPDATE = 1,
    INVALID = 3
}
