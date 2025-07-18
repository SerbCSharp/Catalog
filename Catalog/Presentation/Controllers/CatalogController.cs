using Catalog.Application.Interfaces;
using Catalog.Infrastructure.EventBus.IntegrationEvents;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Presentation.Controllers
{
    [ApiController]
    [Route("[controller]")]

    public class CatalogController : ControllerBase
    {
        private readonly IEventSend _send;

        public CatalogController(IEventSend send)
        {
            _send = send ?? throw new ArgumentNullException(nameof(IEventSend));
        }

        [HttpPost("SendProductPriceChanged")]
        public IActionResult SendProductPriceChanged(ProductPriceChanged @event)
        {
            _send.SendEvent(@event);
            return NoContent();
        }

        [HttpPost("SendProductAdd")]
        public IActionResult SendProductAdd(ProductAdd @event)
        {
            _send.SendEvent(@event);
            return NoContent();
        }
    }
}
