using Microsoft.EntityFrameworkCore;

namespace KmaOoad18.Assignments.Week5.Data
{
    public class LoyaltyContext : DbContext
    {
        public DbSet<Customer> Customers { get; set; }
        public DbSet<SpecialOffering> SpecialOfferings { get; set; }
        public DbSet<Product> Products { get; set; }
        public LoyaltyContext(DbContextOptions<LoyaltyContext> options) : base(options) { }

        public LoyaltyContext() { }
    }
}
