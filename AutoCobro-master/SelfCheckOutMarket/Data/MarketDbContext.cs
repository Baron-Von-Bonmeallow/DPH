using Microsoft.EntityFrameworkCore;

namespace SelfCheckOutMarket.Data
{
    public class MarketDbContext : DbContext
    {
        public MarketDbContext(DbContextOptions<MarketDbContext> options) : base(options) { }
        public DbSet<Models.Product> Products { get; set; } 
        public DbSet<Models.Sales> Sales { get; set; }
        public DbSet<Models.SalesItem> SalesItems { get; set; }
    }
}
