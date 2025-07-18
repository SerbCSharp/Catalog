using RabbitMQ.Client;

namespace Catalog.Infrastructure.EventBus.RabbitMQ
{
    public interface IChannelProvider
    {
        IModel GetChannel();
    }
}
