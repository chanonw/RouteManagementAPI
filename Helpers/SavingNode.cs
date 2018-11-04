namespace RouteAPI.Helpers
{
    public class SavingNode
    {
        private CustomerOrder from;
        private CustomerOrder to;
        public double savings;
        public SavingNode(CustomerOrder c1, CustomerOrder c2, double savings)
        {
            this.from = c1;
            this.to = c2;
            this.savings = savings;
        }
        public CustomerOrder getFrom()
        {
            return this.from;
        }

        public CustomerOrder getTo()
        {
            return this.to;
        }

        public double getSavings()
        {
            return this.savings;
        }
    }
}