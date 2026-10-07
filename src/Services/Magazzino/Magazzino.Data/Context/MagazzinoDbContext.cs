using Microsoft.EntityFrameworkCore;
using Magazzino.Data.Entities;

namespace Magazzino.Data.Context;

public class MagazzinoDbContext : DbContext
{
    public MagazzinoDbContext(DbContextOptions<MagazzinoDbContext> options) : base(options)
    {
    }

    public DbSet<Stock> Stocks => Set<Stock>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Stock>(entity =>
        {
            entity.ToTable("stock");

            entity.HasKey(s => s.ProductId);

            entity.Property(s => s.QuantitaDisponibile)
                .IsRequired();

            entity.Property(s => s.QuantitaRiservata)
                .IsRequired()
                .HasDefaultValue(0);

            entity.Property(s => s.UltimoAggiornamento)
                .IsRequired();

            // Vincolo a livello database: le quantità non possono mai essere negative
            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Stock_QuantitaDisponibile_NonNegativa",
                "\"QuantitaDisponibile\" >= 0"));

            entity.ToTable(t => t.HasCheckConstraint(
                "CK_Stock_QuantitaRiservata_NonNegativa",
                "\"QuantitaRiservata\" >= 0"));
        });

        base.OnModelCreating(modelBuilder);
    }
}