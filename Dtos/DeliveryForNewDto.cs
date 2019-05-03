using System;

namespace RouteAPI.Dtos
{
    public class DeliveryForNewDto
    {
        public string cusCode { get; set; }
        public DateTime transDate { get; set; }
        public int quantity { get; set; }
    }
}