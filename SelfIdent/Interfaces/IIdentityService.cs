using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Account.Deletion;
using SelfIdent.Account.Lock;
using SelfIdent.Account.SecurityContextInvalidation;
using SelfIdent.Account.Update;
using SelfIdent.Identity;
using SelfIdent.Identity.Results;

namespace SelfIdent.Interfaces;

internal interface IIdentityService
{
    IdentityCreationResult CreateIdentity(Account.Registration.RegistrationPayload payload);

    IdentityAuthenticationResult AuthenticateIdentity(Account.Authentication.AuthenticationPayload payload);

    IdentityUpdateResult UpdateIdentity(UpdatePayload payload);

    IdentityDeletionResult DeleteIdentity(DeletionPayload payload);

    IdentityLockResult LockIdentity(LockPayload payload);

    IdentityUnlockResult UnlockIdentity(UnlockPayload payload);

    List<UserIdentity> GetUsers();

    UserIdentity GetUser(ulong id);

    IdentityUpdateResult InvalidateSecurityContext(SecurityContextInvalidationPayload payload);
}
