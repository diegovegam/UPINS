using Microsoft.EntityFrameworkCore;
using UPINS.Models.Domain;

namespace UPINS.Data
{
    public class UpinsDBContext : DbContext
    {
        public UpinsDBContext(DbContextOptions<UpinsDBContext> options) : base(options)
        {
        }

        public DbSet<Product> Product { get; set; }
        public DbSet<Bill> Bill { get; set; }

        public DbSet<BillProduct> BillProduct { get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BillProduct>()
                .HasKey(bp => new { bp.BillId, bp.ProductId }); // Composite key

            modelBuilder.Entity<BillProduct>()
                .HasOne(bp => bp.Bill)
                .WithMany(b => b.Products)
                .HasForeignKey(bp => bp.BillId);

            modelBuilder.Entity<BillProduct>()
                .HasOne(bp => bp.Product)
                .WithMany(p => p.Bills)
                .HasForeignKey(bp => bp.ProductId);
        }
    }
}
