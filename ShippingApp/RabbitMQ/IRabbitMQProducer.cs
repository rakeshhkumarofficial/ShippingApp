namespace ShippingApp.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        public void SendProductMessage<T>(T message);
        public void SendDriverMessage<T>(T message);
    }
}
