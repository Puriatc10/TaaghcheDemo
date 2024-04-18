using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using StackExchange.Redis;
using TaaghcheDemo.Settings;

namespace TaaghcheDemo.CacheServices
{
    public class RedisCacheService : ICacheService
    {
        private readonly IConnectionMultiplexer _redisCache;
        private readonly RedisCacheSettings _options;

        public RedisCacheService(IConnectionMultiplexer redisCache, IOptions<RedisCacheSettings> options)
        {
            _redisCache = redisCache;
            _options = options.Value;
        }

        public string? GetValue(int bookId)
        {
            try
            {
                var db = _redisCache.GetDatabase();
                var key = CacheKeyGenerate(bookId);
                var value = db.StringGet(key);
                if (value.IsNullOrEmpty)
                    return null;
                return value.ToString();
            } catch (Exception ex)
            {
                return null;
            }
        }

        public void SetValue(int bookId, string value)
        {
            try
            {
                var db = _redisCache.GetDatabase();
                var key = CacheKeyGenerate(bookId);
                //db.HashSet("HashBook" , new HashEntry[]{ new HashEntry(key , value)});
                db.StringSet(key, value, _options.DefaultExpirationTime);
                //await db.StringSetAsync(key, value);
            } catch (Exception ex)
            {
                return;
            }
        }

        public void Delete(int bookId)
        {
            try
            {
                var db = _redisCache.GetDatabase();
                var key = CacheKeyGenerate(bookId);
                db.KeyDelete(key);
            }
            catch (Exception ex)
            {
                return;
            }
        }


        private string CacheKeyGenerate(int bookId)
        {
            return $"book-{bookId}";
        }
    }
}
