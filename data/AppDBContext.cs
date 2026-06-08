using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using JewelryEcommerce.Domain;

namespace JewelryEcommerce.Data;

public class AppDBContext : DbContext
{
    public AppDBContext(DbContextOptions<AppDBContext> options) : base(options)
    {
    }

    public DbSet<Product> Products { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuração de Product
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Description)
                .IsRequired();

            entity.Property(e => e.Price)
                .HasPrecision(18, 2);

            entity.Property(e => e.OriginalPrice)
                .HasPrecision(18, 2);

            entity.Property(e => e.Rating)
                .HasPrecision(5, 2);

            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500);

            entity.Property(e => e.Category)
                .HasMaxLength(100);

            entity.Property(e => e.Material)
                .HasMaxLength(100);

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.IsFeatured);
        });

        // Configuração de Order
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.CustomerEmail)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.CustomerPhone)
                .HasMaxLength(20);

            entity.Property(e => e.ShippingAddress)
                .IsRequired();

            entity.Property(e => e.Total)
                .HasPrecision(18, 2);

            entity.Property(e => e.Status)
                .HasDefaultValue(OrderStatus.Pending)
                .HasConversion(
                    v => v.ToString(),
                    v => (OrderStatus)Enum.Parse(typeof(OrderStatus), v))
                .HasColumnType("text");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.CustomerEmail);

            entity.HasMany(e => e.Items)
                .WithOne(o => o.Order)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração de OrderItem
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(255);

            entity.Property(e => e.Price)
                .HasPrecision(18, 2);

            entity.HasOne<Product>()
                .WithMany()
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    }
}
