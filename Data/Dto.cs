using RouteAPI.Models;

namespace RouteAPI.Data
{
    public class Dto
    {
        public string warehouseId { get; set; }
        public string zoneId { get; set; }

        public string[] deliveryId { get; set; }
        public Customer[] customer { get; set; }

        public string carCode {get; set;}

        public string transDate {get; set;}

        public bool pLeave { get; set; }

    }
}