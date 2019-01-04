using System;

namespace RouteAPI.Dtos
{
    public class DeliveryForChangeDateDto
    {
        public string deliveryId { get; set; }
        public string transDate { get; set; }
        public string cusCode { get; set; }
        public string status { get; set; }
    }
}