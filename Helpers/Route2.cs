using System.Collections.Generic;
namespace RouteAPI.Helpers
{
    public class Route2
    {
        private Customer2 start;
        private Customer2 end;
        private List<Customer2> route;
        private Customer2 depot;
        public Route2(Customer2 a, Customer2 dep)
        {
            this.start = a;
            this.end = a;
            this.route = new List<Customer2>();
            this.route.Add(a);
            this.depot = dep;
        }
        public Customer2 getDepot()
        {
            return depot;
        }

        public void setDepot(Customer2 depot)
        {
            this.depot = depot;
        }

        public Customer2 getStart()
        {
            return this.start;
        }

        public void setStart(Customer2 start)
        {
            this.start = start;
        }

        public Customer2 getEnd()
        {
            return this.end;
        }

        public void setEnd(Customer2 end)
        {
            this.end = end;
        }
        public List<Customer2> getRoute()
        {
            return this.route;
        }

        public void setRoute(List<Customer2> route)
        {
            this.route = route;
            this.start = route[0];
            this.end = route[route.Count - 1];
        }

        public void addRoute(Route2 toAdd)
        {
            this.route.AddRange(toAdd.getRoute());
            this.end = this.route[this.route.Count - 1];
        }

        public void addCustomer(Customer2 c)
        {
            this.route.Add(c);
            this.end = c;
        }
        
    }
}