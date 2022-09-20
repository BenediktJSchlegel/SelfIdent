using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Token
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SelfIdentAllowAnonymousAttribute : Attribute
    {
        // No logic required. Only serves to cancel out the SelfIdentTokenAuthorizeAttribute
    }
}
