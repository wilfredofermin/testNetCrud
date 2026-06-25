using Microsoft.EntityFrameworkCore;
using testNet.Domain.Entities;
using testNet.Domain.ValueObjects;

namespace testNet.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products => Set<Product>();

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Ignore<Price>();
        modelBuilder.Ignore<ProductCode>();

        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.CodeValue)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(e => e.Description)
                .HasMaxLength(2000);

            entity.Property(e => e.StockQuantity)
                .IsRequired();

            entity.Property(e => e.IsActive)
                .IsRequired();

            entity.Ignore(e => e.Code);
            entity.Ignore(e => e.UnitPrice);

            entity.HasIndex(e => e.CodeValue)
                .IsUnique();
        });
    }
}
