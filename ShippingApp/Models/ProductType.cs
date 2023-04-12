namespace ShippingApp.Models
{
    public class ProductType
    {
        public Guid productTypeId { get; set; }
        public string type { get; set; }
        public float price { get; set; } = -1;
        public bool isFragile { get; set; } 
    }
}
