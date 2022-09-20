using System;
using Microsoft.Extensions.Caching.Memory;
using SelfIdent.Identity;
using SelfIdent.Options;

namespace SelfIdent.Cache;

internal class SelfIdentMemoryCache
{
    private MemoryCache _cache;
    private CacheOptions _options;

    public SelfIdentMemoryCache(CacheOptions options)
    {
        var memoryOptions = new MemoryCacheOptions()
        {
            SizeLimit = options.SizeLimit,
            CompactionPercentage = options.CompactionPercentage,
            ExpirationScanFrequency = options.ExpirationScanFrequency
        };

        _options = options;
        _cache = new MemoryCache(memoryOptions);
    }

    public UserIdentity? Get(ulong id)
    {
        UserIdentity entry;

        if (_cache.TryGetValue(id, out entry))
            return entry;

        return null;
    }

    public void Create(UserIdentity value)
    {
        var cacheEntryOptions = new MemoryCacheEntryOptions()
                                    .SetSize(1)
                                    .SetPriority(CacheItemPriority.Normal)
                                    .SetSlidingExpiration(_options.SlidingExpiration)
                                    .SetAbsoluteExpiration(_options.AbsoluteExpiration);

        _cache.Set(value, cacheEntryOptions);
    }

}
