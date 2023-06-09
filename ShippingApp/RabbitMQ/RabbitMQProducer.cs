﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace ShippingApp.RabbitMQ
{
    public class RabbitMQProducer : IRabbitMQProducer
    {
        private readonly IConfiguration _configuration;

        public RabbitMQProducer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void SendDriverMessage<T>(T message)
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
            channel.QueueDeclare("notifyDriver", durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);
            //Serialize the message
            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            //put the data on to the product queue
            channel.BasicPublish(exchange: "", routingKey: "notifyDriver", body: body);

        }
        public void SendStatusMessage<T>(T message)
        {
            //Rabbit MQ Server
            var factory = new ConnectionFactory
            {
                //HostName = "localhost"
                HostName = _configuration.GetSection("RabbitMQ:Host").Value!,
                Port = Protocols.DefaultProtocol.DefaultPort,
                UserName = _configuration.GetSection("RabbitMQ:Username").Value!,
                Password = _configuration.GetSection("RabbitMQ:Password").Value!,
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
