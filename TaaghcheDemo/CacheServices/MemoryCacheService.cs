using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using TaaghcheDemo.Settings;

namespace TaaghcheDemo.CacheServices
{
    public class MemoryCacheService : ICacheService
    {
        private readonly IMemoryCache _cache;
        private readonly MemoryCacheSettings _options;

        public MemoryCacheService(IMemoryCache cache, IOptions<MemoryCacheSettings> options)
        {
            _cache = cache;
            _options = options.Value;
        }

        public string? GetValue(int bookId)
        {
            var key = CacheKeyGenerate(bookId);
            if (!_cache.TryGetValue(key, out var value))
            {
                return null;
            }
            return value.ToString();
        }

        // set  default expiration time for each cached value
        public void SetValue(int bookId , string value)
        {
            var key = CacheKeyGenerate(bookId);
            if(_cache.TryGetValue(key, out var preValue))
            {
                _cache.Remove(key);
            }
            _cache.Set(key, value, new MemoryCacheEntryOptions() { SlidingExpiration = _options.DefaultExpirationTime});
        }

        public void Delete(int bookId)
        {
            var key = CacheKeyGenerate(bookId);
            _cache.Remove(key);
        }

        // can set expiration time for each cached value seperately
        public void SetValue(int bookId, object value, MemoryCacheEntryOptions options)
        {
            var key = CacheKeyGenerate(bookId);
            if (_cache.TryGetValue(key, out var preValue))
            {
                _cache.Remove(key);
            }
            _cache.Set(key, value, options);
            return;
        }


        private string CacheKeyGenerate(int bookId)
        {
            return $"book-{bookId}";
        }
    }
}
