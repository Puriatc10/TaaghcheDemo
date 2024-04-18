using MassTransit;
using MessagingContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MessagePublisher.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PublishController : ControllerBase
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public PublishController(IPublishEndpoint bus)
        {
            _publishEndpoint = bus;
        }

        [HttpPost("PublishSomeMessage/{bookId}")]
        public async Task<IActionResult> PublishTestMessage(int bookId)
        {
            var busControl = Bus.Factory.CreateUsingRabbitMq(cfg =>
            {
                cfg.Host("localhost", "/", h =>
                {
                    h.Username("guest");
                    h.Password("guest");
                });
            });

            await busControl.StartAsync();

            try
            {
                var endpoint = await busControl.GetSendEndpoint(new Uri("queue:BookUpdated"));
                await endpoint.Send(new BookUpdated() { BookId = bookId, EventTime = DateTime.Now });
            }
            finally
            {
                await busControl.StopAsync();
            }

            //var message = new BookUpdated() { BookId = bookId, EventTime = DateTime.Now };
            //await _publishEndpoint.Publish(message);
            return Ok();
        }
    }
}
