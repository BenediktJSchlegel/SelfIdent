using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Account.Registration;
using SelfIdent.Validation;

namespace SelfIdent.Interfaces;

public interface IValidator
{
    ValidationResult Validate(RegistrationPayload payload);
}
