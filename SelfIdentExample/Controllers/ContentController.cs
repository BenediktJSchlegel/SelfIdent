using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SelfIdent.Interfaces;

namespace SelfIdentityExample.Controllers
{
    [Authorize]
    public class ContentController : Controller
    {
        private ISelfIdentEndpoints _selfIdent;

        public ContentController(ISelfIdentEndpoints selfIdent)
        {
            _selfIdent = selfIdent;
        }

        public IActionResult RestrictedContent()
        {
            return View();
        }

    }
}
