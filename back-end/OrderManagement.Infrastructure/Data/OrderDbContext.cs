using Microsoft.EntityFrameworkCore;
using OrderManagement.Domain.Entities;
using System.Reflection.Emit;

namespace OrderManagement.Infrastructure.Data;

public class OrderDbContext : DbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options)
        : base(options)
    {
    }

    public DbSet<Order> Orders => Set<Order>();

    public DbSet<OrderLine> OrderLines => Set<OrderLine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(o => o.Id);

            entity.Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(o => o.ExternalReference)
                .IsRequired()
                .HasMaxLength(100);

            entity.HasIndex(o => o.ExternalReference)
                .IsUnique();

            entity.Property(o => o.Status)
                .HasConversion<string>();

            entity.HasMany(o => o.Lines)
                .WithOne()
                .HasForeignKey(line => line.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.Ignore(o => o.Total);

            entity.Navigation(o => o.Lines)
                .UsePropertyAccessMode(PropertyAccessMode.Field);
        });

        modelBuilder.Entity<OrderLine>(entity =>
        {
            entity.HasKey(line => line.Id);

            entity.Property(line => line.ProductName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(line => line.UnitPrice)
                .HasPrecision(18, 2);

            entity.Ignore(line => line.LineTotal);
        });
    }
}