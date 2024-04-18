
using Microsoft.Extensions.Options;
using TaaghcheDemo.CacheServices;
using TaaghcheDemo.Infrastructure;
using TaaghcheDemo.Settings;

namespace TaaghcheDemo.Services
{
    public class BookService : IBookService
    {
        private readonly IApiService _apiService;
        private readonly CacheHandler<MemoryCacheService> _memoryCacheHandler;
        private readonly MemoryCacheService _memoryCacheService;
        private readonly CacheHandler<RedisCacheService> _redisCacheHandler;
        private readonly RedisCacheService _redisCacheService;
        private readonly string _apiUrl;
        public BookService(IApiService apiService,
            CacheHandler<MemoryCacheService> memoryCacheHandler,
            CacheHandler<RedisCacheService> redisCacheHandler,
            IOptions<ApiSettings> options) 
        {
            _apiService = apiService;
            _memoryCacheHandler = memoryCacheHandler;
            _redisCacheHandler = redisCacheHandler;
            _memoryCacheService = _memoryCacheHandler.CacheService;
            _redisCacheService = _redisCacheHandler.CacheService;
            _apiUrl = options.Value.ApiUrl;
        }

        public async Task<string> GetBookInfo(int bookId)
        {
            var memoryCacheResponse = _memoryCacheService.GetValue(bookId);
            if (memoryCacheResponse != null)
            {
                return memoryCacheResponse;
            }
            var redisCacheResponse = _redisCacheService.GetValue(bookId);
            if (redisCacheResponse != null)
            {
                return redisCacheResponse;
            }

            var apiResponse = await _apiService.GetJsonDataAsString(_apiUrl, bookId);

            _memoryCacheService.SetValue(bookId, apiResponse);
            _redisCacheService.SetValue(bookId, apiResponse);

            return apiResponse;
        }
    }
}
