namespace RouteAPI.Helpers
{
    public class DeliveryOrder
    {
        public string deliveryId { get; set; }
        public double distanceToRefPoint { get; set; }
        public int quantity { get; set; }
        public string gps { get; set; }
        public string cusCode { get; set; }
        public DeliveryOrder(string deliveryId, double distanceToRefPoint, int quantity, string gps, string cusCode)
        {
            this.deliveryId = deliveryId;
            this.distanceToRefPoint = distanceToRefPoint;
            this.quantity = quantity;
            this.gps = gps;
            this.cusCode = cusCode;
        }
    }
}