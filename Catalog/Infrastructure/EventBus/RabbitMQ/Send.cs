using Catalog.Application.Interfaces;
using Catalog.Infrastructure.EventBus.IntegrationEvents;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Catalog.Infrastructure.EventBus.RabbitMQ
{
    public class Send : IEventSend
    {
        readonly RabbitMqConfiguration _rabbitMqConfiguration;
        readonly IChannelProvider _channelProvider;
        IModel _channel;
        readonly string _exchange;

        public Send(IOptions<RabbitMqConfiguration> rabbitMqConfiguration, IChannelProvider channelProvider)
        {
            _rabbitMqConfiguration = rabbitMqConfiguration.Value ?? throw new ArgumentNullException(nameof(rabbitMqConfiguration));
            _channelProvider = channelProvider ?? throw new ArgumentNullException(nameof(_channelProvider));
            _exchange = _rabbitMqConfiguration.Exchange ?? throw new ArgumentNullException(nameof(rabbitMqConfiguration));

            _channel = _channelProvider.GetChannel();
            _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Direct, durable: true);
        }

        public void SendEvent(IntegrationEvent @event)
        {
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(@event));
            Console.WriteLine($"Send {_channel.ChannelNumber}");
            _channel.BasicPublish(exchange: _exchange, routingKey: @event.GetType().Name, body: body);
        }
    }
}
