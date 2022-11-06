using Microsoft.AspNetCore.Mvc;
using SelfIdent.Account.Registration;
using SelfIdent.Token;
using SelfIdentExample.API.DTO;

namespace SelfIdentExample.API.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthenticationController : Controller
    {
        private readonly SelfIdent.SelfIdent _selfIdent;

        public AuthenticationController(SelfIdent.SelfIdent ident)
        {
            this._selfIdent = ident;
        }

        [HttpGet]
        [Route("validate")]
        [SelfIdentTokenAuthorizeAttribute]
        public HttpResponseMessage Validate()
        {
            return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public string Authenticate([FromBody] AuthenticationDTO authData)
        {
            if (String.IsNullOrEmpty(authData.Username) || String.IsNullOrEmpty(authData.Password))
                throw new ArgumentException("AuthenticationData not filled");

            var payload = new SelfIdent.Account.Authentication.AuthenticationPayload(HttpContext, authData.Username, authData.Username, authData.Password);

            SelfIdent.Account.Authentication.TokenAuthenticationResult result = _selfIdent.TokenAuthenticate(payload);

            if (!result.Successful || result.Token == null || result.Token.AccessToken == null)
                throw result.ThrownException ?? new Exception("Failed authenticating");

            return result.Token.AccessToken;
        }

        [HttpPost]
        [Route("register")]
        public ulong Register([FromBody] RegistrationDTO registrationData)
        {
            if (String.IsNullOrEmpty(registrationData.Username) || String.IsNullOrEmpty(registrationData.Password))
                throw new ArgumentException("RegistrationData not filled");

            var payload = new RegistrationPayload(HttpContext, registrationData.Username, registrationData.Username, registrationData.Password);

            RegistrationResult result = _selfIdent.Register(payload);

            if (!result.Successful || result.GeneratedUser == null)
                throw result.ThrownException ?? new Exception("Failed registration");

            return result.GeneratedUser.Id;
        }
    }
}