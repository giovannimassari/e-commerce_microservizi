using Microsoft.EntityFrameworkCore;
using Pagamenti.Repository.Entities;

namespace Pagamenti.Repository;

public class PagamentiDbContext(DbContextOptions<PagamentiDbContext> options) : DbContext(options)
{
    public DbSet<Pagamento> Pagamenti => Set<Pagamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pagamento>(entity =>
        {
            entity.ToTable("pagamenti");
            entity.HasKey(p => p.Id);
            entity.Property(p => p.Importo).HasPrecision(18, 2);
            entity.Property(p => p.Stato).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.Property(p => p.Motivo).HasMaxLength(500);
            // Un solo pagamento per ordine: protegge anche dalle riconsegne dello stesso messaggio Kafka
            entity.HasIndex(p => p.OrdineId).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}
