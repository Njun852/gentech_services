using Microsoft.EntityFrameworkCore;
using gentech_services.Models;

namespace gentech_services.Data
{
    public class GentechDbContext : DbContext
    {
        public GentechDbContext(DbContextOptions<GentechDbContext> options) : base(options)
        {
        }

        // DbSets
        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }
        public DbSet<ProductOrderItem> ProductOrderItems { get; set; }
        public DbSet<ServiceOrder> ServiceOrders { get; set; }
        public DbSet<ServiceOrderItem> ServiceOrderItems { get; set; }
        public DbSet<InventoryLog> InventoryLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints

            // Category - Product relationship (One-to-Many)
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // Category - Service relationship (One-to-Many)
            modelBuilder.Entity<Service>()
                .HasOne(s => s.Category)
                .WithMany(c => c.Services)
                .HasForeignKey(s => s.CategoryID)
                .OnDelete(DeleteBehavior.Restrict);

            // ProductOrder - ProductOrderItem relationship (One-to-Many)
            modelBuilder.Entity<ProductOrderItem>()
                .HasOne(poi => poi.ProductOrder)
                .WithMany(po => po.ProductOrderItems)
                .HasForeignKey(poi => poi.ProductOrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // Product - ProductOrderItem relationship (One-to-Many)
            modelBuilder.Entity<ProductOrderItem>()
                .HasOne(poi => poi.Product)
                .WithMany(p => p.ProductOrderItems)
                .HasForeignKey(poi => poi.ProductID)
                .OnDelete(DeleteBehavior.Restrict);

            // ServiceOrder - ServiceOrderItem relationship (One-to-Many)
            modelBuilder.Entity<ServiceOrderItem>()
                .HasOne(soi => soi.ServiceOrder)
                .WithMany(so => so.ServiceOrderItems)
                .HasForeignKey(soi => soi.ServiceOrderID)
                .OnDelete(DeleteBehavior.Cascade);

            // Service - ServiceOrderItem relationship (One-to-Many)
            modelBuilder.Entity<ServiceOrderItem>()
                .HasOne(soi => soi.Service)
                .WithMany(s => s.ServiceOrderItems)
                .HasForeignKey(soi => soi.ServiceID)
                .OnDelete(DeleteBehavior.Restrict);

            // Product - InventoryLog relationship (One-to-Many)
            modelBuilder.Entity<InventoryLog>()
                .HasOne(il => il.Product)
                .WithMany(p => p.InventoryLogs)
                .HasForeignKey(il => il.ProductID)
                .OnDelete(DeleteBehavior.Cascade);

            // User - InventoryLog relationship (One-to-Many)
            modelBuilder.Entity<InventoryLog>()
                .HasOne(il => il.User)
                .WithMany()
                .HasForeignKey(il => il.CreatedBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.SKU)
                .IsUnique();

            // Decimal precision configuration
            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Service>()
                .Property(s => s.Price)
                .HasPrecision(10, 2);

            modelBuilder.Entity<ProductOrder>()
                .Property(po => po.TotalAmount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<ProductOrderItem>()
                .Property(poi => poi.UnitPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<ProductOrderItem>()
                .Property(poi => poi.TotalPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<ServiceOrderItem>()
                .Property(soi => soi.UnitPrice)
                .HasPrecision(10, 2);

            modelBuilder.Entity<ServiceOrderItem>()
                .Property(soi => soi.TotalPrice)
                .HasPrecision(10, 2);
        }
    }
}
