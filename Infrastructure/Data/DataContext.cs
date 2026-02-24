using Domain.Entities;
using Domain.Entities.Products;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) {}

    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Book> Books { get; set; }
    public DbSet<Clothing> Clothes { get; set; }
    public DbSet<Phone> Phones { get; set; }
    
    public DbSet<CartItem> CartItems { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Favourite> Favourites { get; set; }
    public DbSet<ImageStore> ImageStores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Favourite>()
            .HasKey(f => new { f.UserId, f.ProductId });

        modelBuilder.Entity<Product>()
            .HasDiscriminator<string>("ProductType")
            .HasValue<Book>("Book")
            .HasValue<Clothing>("Clothing")
            .HasValue<Phone>("Phone");

        modelBuilder.Entity<CartItem>()
            .HasOne(c => c.Product)
            .WithMany()
            .HasForeignKey(c => c.ProductId)
            .IsRequired(false);

        modelBuilder.Entity<Favourite>()
            .HasOne(f => f.Product)
            .WithMany()
            .HasForeignKey(f => f.ProductId)
            .IsRequired(false);

        modelBuilder.Entity<OrderItem>()
            .HasOne(oi => oi.Product)
            .WithMany()
            .HasForeignKey(oi => oi.ProductId)
            .IsRequired(false);

        modelBuilder.Entity<Product>().HasQueryFilter(p => !p.IsDeleted);
            
        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        modelBuilder.Entity<OrderItem>().Property(oi => oi.PriceAtPurchase).HasPrecision(18, 2);
        modelBuilder.Entity<Order>().Property(o => o.TotalAmount).HasPrecision(18, 2);
    }
}
