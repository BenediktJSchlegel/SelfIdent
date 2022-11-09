using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Constants;
using SelfIdent.DatabaseServices;

namespace SelfIdent.Helpers;

internal static class Helper
{
    public static T? SafelySet<T>(object value)
    {
        if (value == null || value == DBNull.Value)
            return default(T);

        return (T)Convert.ChangeType(value, typeof(T));
    }

    /// <summary>
    /// Tests the DatabaseResult for Exceptions and Returned Object
    /// </summary>
    /// <typeparam name="T">Type of the Returned Object</typeparam>
    /// <param name="result"></param>
    /// <param name="fallback"></param>
    /// <returns></returns>
    public static T CheckDatabaseResult<T>(DatabaseResult result, Exception fallback)
    {
        T? content = GetReturnedObject<T>(result);

        if (!result.Successful || (content == null || !(content is T)))
        {
            if (result.ThrownException != null)
                throw result.ThrownException;

            throw fallback;
        }

        return (T)content;
    }

    private static T? GetReturnedObject<T>(DatabaseResult result)
    {
        object? obj = null;

        switch (result)
        {
            case IdentityDatabaseResult identityDatabaseResult:
                if (identityDatabaseResult.Identity != null)
                    obj = identityDatabaseResult.Identity;
                break;
            case IdentitiesDatabaseResult identitiesDatabaseResult:
                if (identitiesDatabaseResult.Identities != null)
                    obj = identitiesDatabaseResult.Identities;
                break;
            case RoleDatabaseResult roleDatabaseResult:
                if (roleDatabaseResult.Role != null)
                    obj = roleDatabaseResult.Role;
                break;
            case RolesDatabaseResult rolesDatabaseResult:
                if (rolesDatabaseResult.Roles != null)
                    obj = rolesDatabaseResult.Roles;
                break;
            default:
                throw new NotImplementedException();
        }

        if (obj == null)
            return (T?)obj;

        if (!(obj is T))
            throw new Exception($"Returned Object is not of expected type: {typeof(T)}");

        return (T)obj;
    }
}
