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
            _exchange = _rabbitMqConfiguration.Exchange ?? throw new ArgumentNullException(nameof(rabbitMqConfiguration));
            _queue = _rabbitMqConfiguration.Queue ?? throw new ArgumentNullException(nameof(rabbitMqConfiguration));

            _channel = _channelProvider.GetChannel();
            DeclareQueueAndDeadLetter();
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var @event = Encoding.UTF8.GetString(body);
                Console.WriteLine($" [x] Received {@event}");
                Console.WriteLine($"Receive {_channel.ChannelNumber}");
                _channel.BasicAck(ea.DeliveryTag, false);
                return Task.CompletedTask;
            };

            _channel.BasicConsume(queue: _queue, autoAck: false, consumer: consumer);
            return Task.CompletedTask;
        }

        private void DeclareQueueAndDeadLetter()
        {
            var deadLetterQueueName = $"{_queue}-deadletter";

            // Declare the DeadLetter Queue
            var deadLetterQueueArgs = new Dictionary<string, object>
            {
                { "x-queue-type", "quorum" },
                { "overflow", "reject-publish" }
            };
            _channel.ExchangeDeclare(exchange: deadLetterQueueName, type: ExchangeType.Direct);
            _channel.QueueDeclare(queue: deadLetterQueueName, durable: true, exclusive: false, autoDelete: false, arguments: deadLetterQueueArgs);
            _channel.QueueBind(queue: deadLetterQueueName, exchange: deadLetterQueueName, routingKey: deadLetterQueueName);

            // Declare the Queue
            var queueArgs = new Dictionary<string, object>
            {
                { "x-dead-letter-exchange", deadLetterQueueName },
                { "x-dead-letter-routing-key", deadLetterQueueName },
                { "x-queue-type", "quorum" },
                { "x-dead-letter-strategy", "at-least-once" },
                { "overflow", "reject-publish" }
            };
            _channel.ExchangeDeclare(exchange: _exchange, type: ExchangeType.Direct, durable: true);
            _channel.QueueDeclare(queue: _queue, durable: true, exclusive: false, autoDelete: false, arguments: queueArgs);
            _channel.QueueBind(queue: _queue, exchange: _exchange, routingKey: "ProductPriceChanged");
            _channel.QueueBind(queue: _queue, exchange: _exchange, routingKey: "ProductAdd");
        }
    }
}
