namespace ShippingApp.Models
{
    public class GetShippersRequest
    {
        public Guid driverId { get; set; }
        public GetShippersRequest(Guid id)
        {
            driverId = id;
        }
    }
}
