using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SelfIdentityExample.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using SelfIdent.Interfaces;

namespace SelfIdentityExample.Controllers
{
    public class HomeController : Controller
    {
        private ISelfIdentEndpoints _selfIdent;

        public HomeController(ISelfIdentEndpoints selfIdent)
        {
            _selfIdent = selfIdent;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Account()
        {
            return View();
        }

        public IActionResult Logout()
        {
            LogoutCookies();
            return View("Account");
        }

        public IActionResult Login()
        {
            LoginCookies();
            return View("Index");
        }

        private async void LogoutCookies()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private void LoginCookies()
        {
            // TODO: 
            // Pass in Context with the Payload
            // Handle Everything in the Lib
            // Get Back Result to let the Application handle what happens next


            //var payload = new SelfIdent.Authentication.AuthenticationPayload();
            //payload.Username = "benediktschlegel";
            //payload.Password = "SafePaaSsWord123!";

            //var authenticationResult = (SelfIdent.Authentication.CookieAuthenticationResult)SelfIdent.SelfIdent.Instance.Authenticate(payload);

            //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, authenticationResult.CookieClaimsPrincipal, authenticationResult.AuthenticationProperties);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}



//private async void LoginCookies()
//{

//    var claims = new List<Claim>()
//            {
//                new Claim(ClaimTypes.Name, "TheName"),
//                new Claim("LastChanged", "Testvalue")
//            };

//    var claimsIdentity = new ClaimsIdentity(
//    claims,
//    CookieAuthenticationDefaults.AuthenticationScheme);

//    var authProperties = new AuthenticationProperties()
//    {
//        //AllowRefresh = <bool>,
//        // Refreshing the authentication session should be allowed.

//        //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
//        // The time at which the authentication ticket expires. A 
//        // value set here overrides the ExpireTimeSpan option of 
//        // CookieAuthenticationOptions set with AddCookie.

//        //IsPersistent = true,
//        // Whether the authentication session is persisted across 
//        // multiple requests. When used with cookies, controls
//        // whether the cookie's lifetime is absolute (matching the
//        // lifetime of the authentication ticket) or session-based.

//        //IssuedUtc = <DateTimeOffset>,
//        // The time at which the authentication ticket was issued.

//        //RedirectUri = <string>
//        // The full path or absolute URI to be used as an http 
//        // redirect response value.
//    };

//    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);
//}