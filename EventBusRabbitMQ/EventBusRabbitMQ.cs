using System;
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

        public EventBusRabbitMQ(IServiceProvider serviceProvider, string queueName) {
            _serviceProvider = serviceProvider;
            _queueName = queueName;
        }

        public void Publish(IntegrationEvent @event) {
            var factory = new ConnectionFactory() { HostName = "localhost" };
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

        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T> {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.ExchangeDeclare(exchange: BrokerName, type: ExchangeType.Direct);

            channel.QueueDeclare(queue: _queueName);
            channel.QueueBind(queue: _queueName,
                exchange: BrokerName,
                routingKey: typeof(T).Name);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += async (model, ea) => {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var eventData = JsonConvert.DeserializeObject(message, typeof(T));

                var handlerType = typeof(TH);
                using (var scope = _serviceProvider.CreateScope()) {
                    var handler = scope.ServiceProvider.GetRequiredService(handlerType);
                    await (Task)handlerType.GetMethod("Handle").Invoke(handler, new object[] { eventData });
                }
            };
            channel.BasicConsume(queue: _queueName,
                autoAck: false,
                consumer: consumer);
        }

        public void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T> {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
