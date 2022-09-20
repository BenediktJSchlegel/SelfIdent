using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Token
{
    [AttributeUsage(AttributeTargets.Method)]
    internal class SelfIdentAllowAnonymousAttribute : Attribute
    {
    }
}
