using Microsoft.EntityFrameworkCore;
using ShippingApp.Models;

namespace ShippingApp.Data
{
    public class ShippingDbContext : DbContext
    {
        public ShippingDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<ProductType> ProductTypes { get; set; }
        public DbSet<ContainerType> ContainerTypes { get; set; }
        public DbSet<Driver> Drivers { get; set; }
    }
}
