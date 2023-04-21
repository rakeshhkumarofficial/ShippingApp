using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class SendShipmentModel
    {
        public Guid shipmentId { get; set; } 
        public string productType { get; set; }
        public string containerType { get; set; }
        public float shipmentWeight { get; set; } 
        public Guid origin { get; set; } 
        public Guid Destination { get; set; } 
    }
}
