using System.Collections.Generic;

namespace RouteAPI.Helpers
{
    public class OrderList
    {
        private int index;
        private List<Order> deliveryIdList;
        private double totalDistance;
        public OrderList(List<Order> deliveryIdList, double totalDistance, int index)
        {
            this.deliveryIdList = deliveryIdList;
            this.totalDistance = totalDistance;
            this.index = index;
        }

        public List<Order> getDeliveryIdList()
        {
            return this.deliveryIdList;
        }
        public double getTotalDistance()
        {
            return this.totalDistance;
        }

        public int getIndex()
        {
            return this.index;
        }
    }
}