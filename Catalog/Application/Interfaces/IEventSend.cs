using Catalog.Infrastructure.EventBus.IntegrationEvents;

namespace Catalog.Application.Interfaces
{
    public interface IEventSend
    {
        void SendEvent(IntegrationEvent @event);
    }
}
