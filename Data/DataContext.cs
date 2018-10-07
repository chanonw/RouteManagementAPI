
using Microsoft.EntityFrameworkCore;
using RouteAPI.Models;

namespace RouteAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options)
        {
            
        }
        public DbSet<Car> Car { get; set; }
        public DbSet<Customer> Customer {get; set;}
        public DbSet<Delivery> Delivery { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Zone> Zone { get; set; }
    }
}