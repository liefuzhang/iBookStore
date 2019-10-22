using System;
using System.Net.Sockets;
using System.Text;
using EventBus;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EventBusRabbitMQ
{
    public class EventBusRabbitMQ : IEventBus, IDisposable
    {
        const string BrokerName = "ibookstore_event_bus";

        public void Publish(IntegrationEvent @event) {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.ExchangeDeclare(exchange: BrokerName, type: ExchangeType.Direct);

                channel.BasicPublish(exchange: BrokerName,
                    routingKey: @event.GetType().Name,
                    basicProperties: null);
            }
        }

        public void Subscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T> {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel()) {
                channel.ExchangeDeclare(exchange: BrokerName, type: ExchangeType.Direct);

                var queueName = "Ordering";
                channel.QueueDeclare(queue: queueName);
                channel.QueueBind(queue: queueName,
                    exchange: BrokerName,
                    routingKey: typeof(T).Name);
                
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) => {
                    var body = ea.Body;
                    var message = Encoding.UTF8.GetString(body);
                };
                channel.BasicConsume(queue: queueName,
                    autoAck: false,
                    consumer: consumer);
            }
        }

        public void Unsubscribe<T, TH>() where T : IntegrationEvent where TH : IIntegrationEventHandler<T> {
            throw new NotImplementedException();
        }

        public void Dispose() {
            throw new NotImplementedException();
        }
    }
}
