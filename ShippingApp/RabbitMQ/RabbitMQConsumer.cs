using Microsoft.AspNetCore.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ShippingApp.Models;
using ShippingApp.Services;
using System.Text;

namespace ShippingApp.RabbitMQ
{
    public class RabbitMQConsumer : IHostedService
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly IServiceScopeFactory _scopeFactory;

        public RabbitMQConsumer(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Task.Run(() => Consumer("shipmentDelivery"));

            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            _cancellationTokenSource.Cancel();
            await Task.CompletedTask;
        }
        public void Consumer(string queueName)
        {
                var scope = _scopeFactory.CreateScope();
                var service = scope.ServiceProvider.GetService<IDeliveryService>();
                // Rabbit MQ Server
                var factory = new ConnectionFactory
                {
                    HostName = "192.180.3.63",
                    Port = Protocols.DefaultProtocol.DefaultPort,
                    UserName = "s3",
                    Password = "guest",
                    VirtualHost = "/",
                    ContinuationTimeout = new TimeSpan(10, 0, 0, 0)
                    /*Uri
                    = new Uri("amqp://s2:guest@192.180.3.63:5672")*/
                };
                //RabbitMQ connection using connection factory
                var connection = factory.CreateConnection();
                //channel with session and model
                using
                var channel = connection.CreateModel();
                //declare the queue 
                channel.QueueDeclare(queueName, 
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
                //Set Event object which listen message from chanel which is sent by producer
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    ShipmentDeliveryModel shipmentDelivery = System.Text.Json.JsonSerializer.Deserialize<ShipmentDeliveryModel>(message)!;
                    var res = service!.AddDelivery(shipmentDelivery!);
                    if (res.Data == null)
                    {
                        channel.BasicNack(deliveryTag: eventArgs.DeliveryTag, multiple: false, requeue: true);
                    }
                    else
                    {
                        channel.BasicAck(deliveryTag: eventArgs.DeliveryTag, multiple: false);
                    }
                };
                //read the message
                channel.BasicConsume(queue: queueName, autoAck: false, consumer: consumer);
                Console.ReadLine();
            
        }
    }
}
