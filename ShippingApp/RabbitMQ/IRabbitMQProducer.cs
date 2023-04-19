namespace ShippingApp.RabbitMQ
{
    public interface IRabbitMQProducer
    {
        public void SendStatusMessage<T>(T message);
        public void SendDriverMessage<T>(T message);
    }
}
