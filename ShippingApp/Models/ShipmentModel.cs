using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class ShipmentModel
    {
        [Key]
        public Guid shipmentId { get; set; } = Guid.NewGuid();
        public Guid customerId { get; set; }
        public Guid productTypeId { get; set; }
        public Guid cointainerTypeId { get; set; }
        public float shipmentPrice { get; set; } = 0;
        public float shipmentWeight { get; set; } = 0;
        public string origin { get; set; } = string.Empty;
        public string destination { get; set; } = string.Empty;
        public DateTime dateOfOrder { get; set; } = DateTime.Now;
        public Guid shipmentStatusId { get; set; }
        public string notes { get; set; } = string.Empty;
        public bool isDeleted { get; set; } = false;
        public DateTime lastUpdated { get; set; } = DateTime.Now;
    }
}
