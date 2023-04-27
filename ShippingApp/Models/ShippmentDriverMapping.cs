﻿using System.ComponentModel.DataAnnotations;

namespace ShippingApp.Models
{
    public class ShippmentDriverMapping
    {
        [Key]
        public Guid mapId { get; set; }
        public Guid shipmentId { get; set; }
        public string productType { get; set; }
        public string containerType { get; set; }
        public float shipmentWeight { get; set; } 
        public bool isAccepted { get; set; }
        public bool isActive { get; set; }
        public Guid driverId { get; set; }
        public Guid checkpoint1Id { get; set; }
        public Guid checkpoint2Id { get; set; }
        public DateTime time { get ; set; }
    }
}
