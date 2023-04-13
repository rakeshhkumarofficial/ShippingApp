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
        private readonly IServiceProvider _serviceProvider;
        public RabbitMQConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
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
            using (var scope = _serviceProvider.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<IDeliveryService>();
                var factory = new ConnectionFactory
                {
                    HostName = "localhost"
                };
                //RabbitMQ connection using connection factory
                var connection = factory.CreateConnection();
                //channel with session and model
                using
                var channel = connection.CreateModel();
                //declare the queue 
                channel.QueueDeclare(queueName, exclusive: false);
                //Set Event object which listen message from chanel which is sent by producer
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, eventArgs) =>
                {
                    var body = eventArgs.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine($"message received: {message}");
                    ShipmentModel shipment = System.Text.Json.JsonSerializer.Deserialize<ShipmentModel>(message)!;
                    var res = service!.AddDelivery(shipment);
                };
                //read the message
                channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                Console.ReadLine();
            }
        }
    }
}
