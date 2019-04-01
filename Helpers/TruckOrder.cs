using System.Collections.Generic;

namespace RouteAPI.Helpers
{
    public class TruckOrder
    {
        public string truckCode { get; set; }
        private List<Order> first;
        private List<Order> second;
        private int secondIndex;
        private int firstIndex;
        private double totalDistance;
        public TruckOrder(List<Order> first, List<Order> second, double totalDistance, 
            int firstIndex, int secondIndex, string truckCode)
        {
            this.truckCode = truckCode;
            this.first = first;
            this.second = second;
            this.totalDistance = totalDistance;
            this.firstIndex = firstIndex;
            this.secondIndex = secondIndex;
        }
        public double getTotaldistance()
        {
            return this.totalDistance;
        }
        public int getFirstIndex()
        {
            return this.firstIndex;
        }
        public int getSecondIndex()
        {
            return this.secondIndex;
        }

        public List<Order> getFirstTripOrder()
        {
            return this.first;
        }

        public List<Order> getSecordTripOrder()
        {
            return this.second;
        }
    }
}