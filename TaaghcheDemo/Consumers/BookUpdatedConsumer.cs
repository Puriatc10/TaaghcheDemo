using MassTransit;
using MessagingContract;
using TaaghcheDemo.CacheServices;

namespace TaaghcheDemo.Consumers
{
    public sealed class BookUpdatedConsumer : IConsumer<BookUpdated>
    {

        private readonly ILogger<BookUpdatedConsumer> _logger;
        private readonly CacheHandler<MemoryCacheService> _memoryCacheHandler;
        private readonly MemoryCacheService _memoryCacheService;
        private readonly CacheHandler<RedisCacheService> _redisCacheHandler;
        private readonly RedisCacheService _redisCacheService;

        public BookUpdatedConsumer(ILogger<BookUpdatedConsumer> logger,
            CacheHandler<MemoryCacheService> memoryCacheHandler,
            CacheHandler<RedisCacheService> redisCacheHandler)
        {
            _memoryCacheHandler = memoryCacheHandler;
            _redisCacheHandler = redisCacheHandler;
            _memoryCacheService = _memoryCacheHandler.CacheService;
            _redisCacheService = _redisCacheHandler.CacheService;
            _logger = logger;
        }

        public Task Consume(ConsumeContext<BookUpdated> context)
        {
            _logger.LogInformation("Message Recieved: " + context.Message.ToString());
            _memoryCacheService.Delete(context.Message.BookId);
            _redisCacheService.Delete(context.Message.BookId);

            return Task.CompletedTask;
        }
    }
}
