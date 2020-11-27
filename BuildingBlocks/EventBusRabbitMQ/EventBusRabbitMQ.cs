using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using EventBus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BrokerName = "ibookstore_event_bus";
        private IServiceProvider _serviceProvider;
        private string _queueName;
        private readonly string _mesageQueueUrl;
        private IModel _consumerChannel;
        private IDictionary<string, Type> _eventTypes;
        private IDictionary<Type, Type> _subscriptions;
        private ILogger<EventBusRabbitMQ> _logger;

        public EventBusRabbitMQ(IServiceProvider serviceProvider, string queueName, string mesageQueueUrl) {
            _serviceProvider = serviceProvider;
            _queueName = queueName;
            _mesageQueueUrl = mesageQueueUrl;
            _eventTypes = new Dictionary<string, Type>();
            _subscriptions = new Dictionary<Type, Type>();
            _logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<EventBusRabbitMQ>();

            _consumerChannel = CreateConsumerChannel();
        }

        public void Publish(IntegrationEvent @event) {
            var factory = new ConnectionFactory() { Uri = new Uri(_mesageQueueUrl) };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.ExchangeDeclare(exchange: BrokerName, type: ExchangeType.Direct);

                var message = JsonConvert.SerializeObject(@event);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish(exchange: BrokerName,
                    routingKey: @event.GetType().Name,
                    basicProperties: null,
                    body: body);
            }
        }

        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T>
        {
            var eventTypeName = typeof(T).Name;
            _eventTypes.Add(eventTypeName, typeof(T));
            _subscriptions.Add(typeof(T), typeof(TH));

            _consumerChannel.QueueBind(queue: _queueName,
                exchange: BrokerName,
                routingKey: eventTypeName);
        }

        private IModel CreateConsumerChannel() {
            _logger.LogInformation($"CreateConsumerChannel {_mesageQueueUrl}");

            var factory = new ConnectionFactory() { Uri = new Uri(_mesageQueueUrl) };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: BrokerName, type: ExchangeType.Direct);

            channel.QueueDeclare(queue: _queueName);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) => {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                var eventType = _eventTypes[routingKey];
                var eventData = JsonConvert.DeserializeObject(message, eventType);
                var handlerType = _subscriptions[eventType];
                using (var scope = _serviceProvider.CreateScope()) {
                    var handler = scope.ServiceProvider.GetRequiredService(handlerType);
                    await (Task)handlerType.GetMethod("Handle").Invoke(handler, new object[] { eventData });
                }
            };
            channel.BasicConsume(queue: _queueName,
                autoAck: false,
                consumer: consumer);

            return channel;
        }

        public void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T> {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _consumerChannel?.Dispose();
        }
    }
}
