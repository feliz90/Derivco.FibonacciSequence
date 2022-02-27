using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Derivco.FibonacciSequence.Logic.Cache
{
    public class MemoryCacheService<TEntry, TKey> : IMemoryCacheService<TEntry, TKey>
    {
        private readonly IMemoryCache _cache;
        private readonly IConfiguration _configuration;
        
        public MemoryCacheService(IMemoryCache cache, IConfiguration configuration)
        {
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
            _configuration = configuration;
        }
        
        private TimeSpan CacheLifeTime => TimeSpan.FromMinutes(Convert.ToDouble(_configuration.GetSection("CacheLifeTimeMinutes").Value));
        
        
        public Task<TEntry> GetSequence(TKey key)
        {
            _cache.TryGetValue(key, out TEntry entry);
            return Task.FromResult(entry);
        }
        public void SetSequence(TKey key, TEntry entry) => _cache.Set(key, entry, CacheLifeTime);
        
        public void Remove(TKey key) => _cache.Remove(key);
    }
}