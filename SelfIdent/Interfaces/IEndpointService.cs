using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Account.Authentication;
using SelfIdent.Account.Deletion;
using SelfIdent.Account.Registration;
using SelfIdent.Account.Update;
using SelfIdent.Account.Lock;
using SelfIdent.Account.SecurityContext;
using Microsoft.AspNetCore.Http;
using SelfIdent.Account.SecurityContextInvalidation;

namespace SelfIdent.Interfaces;

public interface ISelfIdentEndpoints
{
    #region Implemented
    RegistrationResult Register(RegistrationPayload payload);

    UpdateResult Update(UpdatePayload payload);

    AuthenticationResult Authenticate(AuthenticationPayload payload);

    DeletionResult Delete(DeletionPayload payload);

    List<User> GetAllUsers();

    User GetUser(ulong id);

    LockResult LockAccount(LockPayload payload);

    UnlockResult UnlockAccount(UnlockPayload payload);

    AuthenticationResult CookieAuthenticate(CookieAuthenticationPayload payload);

    void CookieSignOut(HttpContext context);

    TokenAuthenticationResult TokenAuthenticate(AuthenticationPayload payload);

    #endregion

    #region Not-Implemented

    LockResult PermanentelyLockAccount(LockPayload payload);

    object GeneratePasswordResetKey(object payload);

    SecurityContextInvalidationResult InvalidateSecurityContext(SecurityContextInvalidationPayload payload);

    #endregion

}
