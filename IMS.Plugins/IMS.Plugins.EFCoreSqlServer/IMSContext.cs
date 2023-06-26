using IMS.CoreBusiness;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class IMSContext : DbContext
    {
        public IMSContext(DbContextOptions<IMSContext> options) : base(options)
        {

        }

        public DbSet<Inventory> Inventories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductInventory> ProductInventories { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<ProductTransaction> ProductTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductInventory>(ConfigureProductInventory);
            modelBuilder.Entity<Inventory>(ConfigureInventory);
            modelBuilder.Entity<Product>(ConfigureProduct);

            SeedData(modelBuilder);
        }

        private void ConfigureProductInventory(EntityTypeBuilder<ProductInventory> builder)
        {
            builder.HasKey(pi => new { pi.ProductID, pi.InventoryID });

            builder.HasOne(pi => pi.Product)
                .WithMany(p => p.ProductInventories)
                .HasForeignKey(pi => pi.ProductID);

            builder.HasOne(pi => pi.Inventory)
                .WithMany(i => i.ProductInventories)
                .HasForeignKey(pi => pi.InventoryID);
        }

        private void ConfigureInventory(EntityTypeBuilder<Inventory> builder)
        {
            // Configure Inventory entity properties
            builder.Property(i => i.InventoryName).IsRequired();
            builder.Property(i => i.Quantity).IsRequired();
            builder.Property(i => i.Price).IsRequired();
        }

        private void ConfigureProduct(EntityTypeBuilder<Product> builder)
        {
            // Configure Product entity properties
            builder.Property(p => p.ProductName).IsRequired();
            builder.Property(p => p.Quantity).IsRequired();
            builder.Property(p => p.Price).IsRequired();
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { InventoryID = 1, InventoryName = "Bike Seat", Quantity = 10, Price = 2 },
                new Inventory { InventoryID = 2, InventoryName = "Bike Body", Quantity = 10, Price = 15 },
                new Inventory { InventoryID = 3, InventoryName = "Bike Wheels", Quantity = 20, Price = 8 },
                new Inventory { InventoryID = 4, InventoryName = "Bike Pedals", Quantity = 20, Price = 1 }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { ProductID = 1, ProductName = "Bike", Quantity = 1, Price = 200 },
                new Product { ProductID = 2, ProductName = "PC", Quantity = 15, Price = 300 }
            );

            modelBuilder.Entity<ProductInventory>().HasData(
                new ProductInventory { ProductID = 1, InventoryID = 1, InventoryQuantity = 1 },
                new ProductInventory { ProductID = 1, InventoryID = 2, InventoryQuantity = 1 },
                new ProductInventory { ProductID = 1, InventoryID = 3, InventoryQuantity = 2 },
                new ProductInventory { ProductID = 1, InventoryID = 4, InventoryQuantity = 2 }
            );
        }
    }
}
