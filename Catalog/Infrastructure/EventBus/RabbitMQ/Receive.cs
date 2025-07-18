using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace Catalog.Infrastructure.EventBus.RabbitMQ
{
    public class Receive : BackgroundService
    {
        readonly RabbitMqConfiguration _rabbitMqConfiguration;
        readonly IChannelProvider _channelProvider;

        IModel _channel;


        readonly string _exchange;
        readonly string _queue;

        public Receive(IOptions<RabbitMqConfiguration> rabbitMqConfiguration, IChannelProvider channelProvider)
        {
            _rabbitMqConfiguration = rabbitMqConfiguration.Value ?? throw new ArgumentNullException(nameof(rabbitMqConfiguration));
            _channelProvider = channelProvider ?? throw new ArgumentNullException(nameof(_channelProvider));

            _channel = _channelProvider.GetChannel();


            _exchange = _rabbitMqConfiguration.Exchange ?? throw new ArgumentNullException(nameof(rabbitMqConfiguration));
            _queue = _rabbitMqConfiguration.Queue ?? throw new ArgumentNullException(nameof(rabbitMqConfiguration));
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
                _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Direct, durable: true);
                _channel.QueueDeclare(queue: _queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
                _channel.QueueBind(queue: _queue, exchange: _exchange, routingKey: "ProductPriceChanged");
                _channel.QueueBind(queue: _queue, exchange: _exchange, routingKey: "ProductAdd");

                var consumer = new AsyncEventingBasicConsumer(_channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var @event = Encoding.UTF8.GetString(body);
                    Console.WriteLine($" [x] Received {@event}");
                    _channel.BasicAck(ea.DeliveryTag, false);
                    return Task.CompletedTask;
                };

                _channel.BasicConsume(queue: _queue, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }
    }
}
