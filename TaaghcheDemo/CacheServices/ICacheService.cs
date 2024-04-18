using Microsoft.Extensions.Caching.Memory;

namespace TaaghcheDemo.CacheServices
{
    public interface ICacheService
    {
        public string? GetValue(int bookId);
        public void SetValue(int bookId, string value);
        public void Delete(int bookId);
    }
}
