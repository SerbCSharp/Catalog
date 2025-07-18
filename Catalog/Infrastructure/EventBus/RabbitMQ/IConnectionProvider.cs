using RabbitMQ.Client;

namespace Catalog.Infrastructure.EventBus.RabbitMQ
{
    public interface IConnectionProvider
    {
        IConnection GetConnection();
    }
}
