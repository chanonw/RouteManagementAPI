using System.Collections.Generic;

namespace RouteAPI.Helpers
{
    public class Route
    {
        private List<CustomerOrder> route;
        private int capacity;
        public Route()
        {
            route = new List<CustomerOrder>();
            capacity = 0;
        }
        public void Add(CustomerOrder customer)
        {
            route.Add(customer);
            capacity = capacity + customer.quantity;
        }
        public void Add(int index, CustomerOrder customer)
        {
            route.Insert(index, customer);
            capacity = capacity + customer.quantity;
        }
        public void addAll(Route route)
        {
            this.route.AddRange(route.GetList());
            // also update the capacity
            capacity = capacity + route.getCurrentCapacity();
        }
        public int getCurrentCapacity()
        {
            return capacity;
        }
        public List<CustomerOrder> GetList()
        {
            return route;
        }
        public CustomerOrder getLastCustomer()
        {
            return route[route.Count - 1];
        }
        public CustomerOrder getFirstCustomer()
        {
            return route[0];
        }
        public bool hasCapacity(int requirement, int maxCapacity)
        {
            if ((capacity + requirement) <= maxCapacity)
            {
                return true;
            }
            return false;
        }
    }
}