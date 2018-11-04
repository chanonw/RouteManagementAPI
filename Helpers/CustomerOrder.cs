namespace RouteAPI.Helpers
{
    public class CustomerOrder
    {
        
        public int quantity;
        //public double latitude;
        //public double longtitude;
        public string cusCode;
        public CustomerOrder(int quantity, string cusCode)
        {
            //this.latitude = latitude;
            //this.longtitude = longtitude;
            this.quantity = quantity;
            this.cusCode = cusCode;
        }   
    }
}