namespace ShippingApp.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        public void SendProductMessage<T>(T message);
    }
}
