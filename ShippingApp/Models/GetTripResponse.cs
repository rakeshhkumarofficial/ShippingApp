namespace ShippingApp.Models
{
    public class GetTripResponse
    {
        public int totalTrips { get; set; }
        public float totalEarnings { get; set; }
        public float monthlyEarning { get; set; }
        public float todayEarning { get; set; }
    }
}
