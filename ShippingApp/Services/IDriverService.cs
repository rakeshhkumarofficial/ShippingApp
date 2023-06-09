﻿using ShippingApp.Models;

namespace ShippingApp.Services
{
    public interface IDriverService
    {
        public Response AddDriver(Driver driver);
        public Response DeleteDriver(Guid driverId);
        public Response GetDriver(Guid driverId, Guid checkpointLocation);
        public Response UpdateDriver(UpdateDriverRequest updateDriver);
        public Response GetShippers(Guid checkpointLocation, Guid driverId);
        public Response AcceptShipment(AcceptShipmentRequest request);
        public Response GetDriverEarnings(Guid driverId);
        public Response GetDateEarnings(Guid driverId, DateTime date1, DateTime date2);
        public Response GetChartEarnings(Guid driverId);
    }
}
