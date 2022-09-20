using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfIdent.Options;

public class CacheOptions
{
    /// <summary>
    /// How many Items can be cached at any Time.
    /// -
    /// Each Identity is set to a Size of 1
    /// </summary>
    public long? SizeLimit { get; set; }
    /// <summary>
    /// Amount to compact the cache by when the maximum size is exceeded.
    /// </summary>
    public double CompactionPercentage { get; set; }
    /// <summary>
    /// Minimum length of time between successive scans for expired
    /// </summary>
    public TimeSpan ExpirationScanFrequency { get; set; }
    /// <summary>
    /// The time an item stays cached, refreshed when the item is retrieved.
    /// </summary>
    public TimeSpan SlidingExpiration { get; set; } = TimeSpan.FromMinutes(1);
    /// <summary>
    /// The maximum time an item stays cached.
    /// </summary>
    public TimeSpan AbsoluteExpiration { get; set; } = TimeSpan.FromMinutes(5);
}
