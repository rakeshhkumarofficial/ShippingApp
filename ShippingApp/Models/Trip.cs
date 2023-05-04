using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class Trip
    {
        [Key]
        public Guid tripId { get; set; }
        public Guid driverId { get; set; }
        public Guid checkpoint1Id { get; set;}
        public Guid checkpoint2Id { get; set; }
        public float Price { get; set;}
        public DateTime dateTime { get; set;}

    }
}
