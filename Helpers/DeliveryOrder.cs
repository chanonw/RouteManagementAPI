namespace RouteAPI.Helpers
{
    public class DeliveryOrder
    {
        public string deliveryId { get; set; }
        public double distanceToRefPoint { get; set; }
        public int quantity { get; set; }
        public DeliveryOrder(string deliveryId, double distanceToRefPoint, int quantity)
        {
            this.deliveryId = deliveryId;
            this.distanceToRefPoint = distanceToRefPoint;
            this.quantity = quantity;
        }
    }
}