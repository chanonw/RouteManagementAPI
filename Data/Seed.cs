using System.Collections.Generic;
using RouteAPI.Models;
using Newtonsoft.Json;

namespace RouteAPI.Data
{
    public class Seed
    {
        private readonly DataContext _context;
        public Seed(DataContext context)
        {
            _context = context;

        }
        public void SeedOrder()
        {
            // _context.Delivery.RemoveRange(_context.Delivery);
            // _context.SaveChanges();

            var orderData = System.IO.File.ReadAllText("Data/Order.json");
            var deliveries = JsonConvert.DeserializeObject<List<Delivery>>(orderData);
            _context.Delivery.AddRange(deliveries);
            _context.SaveChanges();
        }
    }
}