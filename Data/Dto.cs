using System.Collections.Generic;
using RouteAPI.Models;

namespace RouteAPI.Data
{
    public class Dto
    {
        public string warehouseId { get; set; }
        public string zoneId { get; set; }

        public string[] deliveryId { get; set; }
        public Customer[] customer { get; set; }

        public string truckCode {get; set;}

        public string transDate {get; set;}

        public bool pLeave { get; set; }

        public IEnumerable<Delivery> deliveries { get; set; }
        public IEnumerable<Truck> trucks { get; set; }

        public double noOfTruck { get; set; }
        public string dayName { get; set; }

    }
}