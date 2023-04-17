using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ShippingApp.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        public void SendProductMessage<T>(T message)
        {
            //Rabbit MQ Server
            var factory = new ConnectionFactory
            {
                //HostName = "localhost"
                HostName = "192.180.3.63",
                Port = Protocols.DefaultProtocol.DefaultPort,
                UserName = "s3",
                Password = "guest",
                VirtualHost = "/",
                ContinuationTimeout = new TimeSpan(10, 0, 0, 0)
            };
            //RabbitMQ connection using connection factory
            var connection = factory.CreateConnection();
            //channel with session and model
            using
            var channel = connection.CreateModel();
            //declare the queue
            channel.QueueDeclare("shipmentStatus", durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            //Serialize the message
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            //put the data on to the product queue
            channel.BasicPublish(exchange: "", routingKey: "shipmentStatus", body: body);
        }
    }
}
