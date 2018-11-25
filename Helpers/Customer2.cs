using System.Collections.Generic;

namespace RouteAPI.Helpers
{
    public class Customer2
    {
        private int quantity;
        private double latitude;
        private double longtitude;
        private string cusCode;

        public double Latitude { get => latitude; set => latitude = value; }
        public double Longtitude { get => longtitude; set => longtitude = value; }
        public int Quantity { get => quantity; set => quantity = value; }

        public Customer2(double x, double y, int quantity, string cusCode)
        {
            this.Quantity = quantity;
            this.Latitude = x;
            this.Longtitude = y;
            this.cusCode = cusCode;
        }
        public Customer2(double x, double y)
        {
            this.Latitude = x;
            this.Longtitude = y;
        }
        
    }
}