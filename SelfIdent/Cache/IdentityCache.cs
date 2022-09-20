using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SelfIdent.Helpers;
using SelfIdent.Identity;
using SelfIdent.Interfaces;

namespace SelfIdent.Cache;

internal class IdentityCache
{
    private IDatabaseService _databaseService;
    private SelfIdentMemoryCache _memoryCache;

    public IdentityCache(IDatabaseService databaseService, SelfIdentMemoryCache memoryCache)
    {
        this._databaseService = databaseService;
        this._memoryCache = memoryCache;
    }

    public UserIdentity? GetIdentity(ulong id)
    {
        try
        {
            UserIdentity? cached = _memoryCache.Get(id);

            if (cached == null)
            {
                // No Cached Identity
                UserIdentity? db = GetIdentityFromDatabase(id);

                if (db == null)
                    return null;

                _memoryCache.Create(db);

                return db;
            }

            return cached;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private UserIdentity? GetIdentityFromDatabase(ulong id)
    {
        DatabaseServices.IdentityDatabaseResult databaseResult = _databaseService.GetIdentity(id);

        if (databaseResult == null || !databaseResult.Successful || databaseResult.Identity == null)
            return null;

        return databaseResult.Identity;
    }
}
