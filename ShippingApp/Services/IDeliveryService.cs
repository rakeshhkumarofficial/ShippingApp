﻿using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IDeliveryService
    {
        public Response AddDelivery(ShipmentDeliveryModel shipmentDelivery);
       
    }
}
