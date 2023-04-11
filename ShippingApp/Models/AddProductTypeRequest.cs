namespace ShippingApp.Models
{
    public class AddProductTypeRequest
    {
        public string type { get; set; }
        public decimal price { get; set; }
        public bool isFragile { get; set; }
    }
}
