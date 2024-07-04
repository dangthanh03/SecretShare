using Microsoft.Extensions.Caching.Memory;
using SecretShare.Service.Abstract;

namespace SecretShare.Service.Implementation
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, Func<Task<T>> createItem)
        {
            if (!_memoryCache.TryGetValue(key, out T cacheEntry))
            {
                cacheEntry = await createItem();
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                    .SetSlidingExpiration(TimeSpan.FromMinutes(5))
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(60));

                _memoryCache.Set(key, cacheEntry, cacheEntryOptions);
            }
            return cacheEntry;
        }

        public void Remove(string key)
        {
            _memoryCache.Remove(key);
        }
    }

}
