using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using SelfIdent.Interfaces;
using SelfIdentExample.ViewModels;

namespace SelfIdentExample.Controllers
{
    public class AccountController : Controller
    {
        private ISelfIdentEndpoints _selfIdent;

        public AccountController(ISelfIdentEndpoints selfIdent)
        {
            _selfIdent = selfIdent;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {

            //for (int i = 0; i < 100; i++)
            //{
            //    System.Diagnostics.Debug.WriteLine("STARTING NEW THREAD " + i);
            //    StartThread(i);
            //}


            return View();
        }

        public IActionResult EditUser(ulong id)
        {
            SelfIdent.User user = _selfIdent.GetUser(id);

            return View("Edit", new EditUserViewModel { User = user });
        }

        private void StartThread(int thread)
        {
            var t = new Thread(() => RunFunc(thread));
            t.Start();
        }

        private void RunFunc(int thread)
        {
            for (int i = 0; i < 100; i++)
            {
                _selfIdent.GetAllUsers();
                System.Diagnostics.Debug.WriteLine("ITERATION " + i + " ON THREAD: " + thread);
            }
        }

        public IActionResult Logout()
        {
            _selfIdent.CookieSignOut(HttpContext);

            return RedirectToAction("Login");
        }

        public IActionResult Registration()
        {
            return View();
        }

        public IActionResult Users()
        {
            System.Collections.Generic.List<SelfIdent.User> result = _selfIdent.GetAllUsers();

            return View("Users", result);
        }

        public IActionResult Lock(ulong id)
        {
            var payload = new SelfIdent.Account.Lock.LockPayload(HttpContext, id);

            SelfIdent.Account.Lock.LockResult result = _selfIdent.LockAccount(payload);

            if (!result.Successful)
            {
                if (result.ThrownException != null)
                    throw result.ThrownException;

                throw new System.Exception("Unknown error locking account.");
            }

            return Users();
        }

        public IActionResult Unlock(ulong id, string key)
        {
            var payload = new SelfIdent.Account.Lock.UnlockPayload(HttpContext, id, key);

            SelfIdent.Account.Lock.UnlockResult result = _selfIdent.UnlockAccount(payload);

            if (!result.Successful)
            {
                if (result.ThrownException != null)
                    throw result.ThrownException;

                throw new System.Exception("Unknown error unlocking account.");
            }

            return Users();
        }

        [HttpPost]
        public IActionResult Edit(SelfIdent.User user)
        {
            var payload = new SelfIdent.Account.Update.UpdatePayload(HttpContext, user.Id);

            payload.Email = user.Email;
            payload.Username = user.Name;
            payload.Password = String.Empty;

            SelfIdent.Account.Update.UpdateResult result = _selfIdent.Update(payload);

            if (result.Successful != null && (bool)result.Successful)
                return RedirectToAction(nameof(Index));
            else
                throw result.ThrownException;
        }

        [HttpPost]
        public IActionResult LoginUser(ViewModels.LoginViewModel model)
        {
            var payload = new SelfIdent.Account.Authentication.CookieAuthenticationPayload(HttpContext, string.Empty, model.Email, model.Password);

            SelfIdent.Account.Authentication.AuthenticationResult result = _selfIdent.CookieAuthenticate(payload);

            if (!result.Successful)
            {
                switch (result.Status)
                {
                    case SelfIdent.Account.Authentication.AuthenticationResultStatus.FAILED_UNKNOWN:
                        throw result.ThrownException;
                    case SelfIdent.Account.Authentication.AuthenticationResultStatus.FAILED_LOCKED:
                        throw new System.Exception("USER IS LOCKED");
                    case SelfIdent.Account.Authentication.AuthenticationResultStatus.FAILED_BADINPUT:
                        throw new System.Exception("BAD INPUT");
                    case SelfIdent.Account.Authentication.AuthenticationResultStatus.FAILED_UNKNOWNUSER:
                        throw new System.Exception("USER IS UNKNOWN");
                    default:
                        break;
                }
            }

            return View("Index");
        }

        [HttpPost]
        public IActionResult Register(ViewModels.RegistrationViewModel model)
        {
            var payload = new SelfIdent.Account.Registration.RegistrationPayload(HttpContext, model.Username, model.Email, model.Password);

            SelfIdent.Account.Registration.RegistrationResult result = _selfIdent.Register(payload);

            if (!result.Successful)
                throw result.ThrownException;

            return View("Index");
        }

        public IActionResult Delete(ulong id)
        {
            var payload = new SelfIdent.Account.Deletion.DeletionPayload(HttpContext, id);

            SelfIdent.Account.Deletion.DeletionResult result = _selfIdent.Delete(payload);

            if (!result.Successful)
                throw result.ThrownException;

            return Users();
        }
    }
}
