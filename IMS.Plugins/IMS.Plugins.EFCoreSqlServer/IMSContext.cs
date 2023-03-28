using IMS.CoreBusiness;
using Microsoft.EntityFrameworkCore;

namespace IMS.Plugins.EFCoreSqlServer
{
    public class IMSContext: DbContext
    {

        public IMSContext(DbContextOptions<IMSContext> options):base(options) 
        { 
            
        }
        public DbSet<Inventory>  Inventories { get; set; }  
        public DbSet<Product> Products { get; set; }  
        public DbSet<ProductInventory> ProductInventories { get; set; }  
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }  
        public DbSet<ProductTransaction> ProductTransactions { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ProductInventory>()
                .HasKey(pi => new { pi.ProductID, pi.InventoryID });

            modelBuilder.Entity<ProductInventory>()
                .HasOne(pi => pi.Product)
                .WithMany(p => p.ProductInventories)
                .HasForeignKey(pi => pi.ProductID);

            modelBuilder.Entity<ProductInventory>()
                .HasOne(pi => pi.Inventory)
                .WithMany(i => i.ProductInventories)
                .HasForeignKey(pi => pi.InventoryID);

            //seeding data
            modelBuilder.Entity<Inventory>().HasData(
                new Inventory { InventoryID = 1, InventoryName = "Bike Seat", Quantity = 10, Price = 2 },
                new Inventory { InventoryID = 2, InventoryName = "Bike Body", Quantity = 10, Price = 15 },
                new Inventory { InventoryID = 3, InventoryName = "Bike Wheels", Quantity = 20, Price = 8 },
                new Inventory { InventoryID = 4, InventoryName = "Bike Pedals", Quantity = 20, Price = 1 }
                );

            modelBuilder.Entity<Product>().HasData(
                 new Product() { ProductID = 1, ProductName = "Bike", Quantity = 1, Price = 200 },
                 new Product() { ProductID = 2, ProductName = "PC", Quantity = 15, Price = 300 }
                );

            modelBuilder.Entity<ProductInventory>().HasData(
                new ProductInventory { ProductID = 1, InventoryID = 1, InventoryQuantity = 1 },//seat
                new ProductInventory { ProductID = 1, InventoryID = 2, InventoryQuantity = 1 },//body
                new ProductInventory { ProductID = 1, InventoryID = 3, InventoryQuantity = 2 },//wheels
                new ProductInventory { ProductID = 1, InventoryID = 4, InventoryQuantity = 2 }//pedals
                );
        }



    }
}