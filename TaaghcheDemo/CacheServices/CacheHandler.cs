namespace TaaghcheDemo.CacheServices
{
    public class CacheHandler<CH> where CH : ICacheService
    {
        public CacheHandler(CH cH)
        {
            CacheService = cH;
        }

        public CH CacheService { get; set; }
    }
}
