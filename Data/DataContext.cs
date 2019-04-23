
using Microsoft.EntityFrameworkCore;
using RouteAPI.Models;

namespace RouteAPI.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }
        public DbSet<Truck> Truck { get; set; }
        public DbSet<AdditionalTruck> AdditionalTruck { get; set; }
        public DbSet<Customer> Customer { get; set; }
        public DbSet<Delivery> Delivery { get; set; }
        public DbSet<Warehouse> Warehouse { get; set; }
        public DbSet<Zone> Zone { get; set; }
        public DbSet<UsedDate> UsedDate { get; set; }
    }
}