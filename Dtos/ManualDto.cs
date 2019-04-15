using System.Collections.Generic;
using RouteAPI.Models;

namespace RouteAPI.Dtos
{
    public class ManualDto
    {
        public IEnumerable<Truck> unUsedTruck { get; set; }
        public  IEnumerable<Truck> selectedTruck { get; set; }
        public string transDate { get; set; }
        public IEnumerable<Delivery> deliveries { get; set; }
    }
}