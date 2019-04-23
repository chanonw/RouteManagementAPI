namespace RouteAPI.Dtos
{
    public class DeliveryForUpdateStatusDto
    {
        public string deliveryId { get; set; }
        public string reason { get; set; }
        public string giveback { get; set; }
        public string coupon { get; set; }
    }
}