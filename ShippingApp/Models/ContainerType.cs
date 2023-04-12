namespace ShippingApp.Models
{
    public class ContainerType
    {
        public Guid containerTypeId { get; set; }
        public string containerName { get; set; }
        public float price { get; set; } = -1;
    }
}
