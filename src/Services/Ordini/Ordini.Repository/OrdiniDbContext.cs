using Microsoft.EntityFrameworkCore;
using Ordini.Business.Domain;

namespace Ordini.Repository;

public class OrdiniDbContext(DbContextOptions<OrdiniDbContext> options) : DbContext(options)
{
    public DbSet<Ordine> Ordini => Set<Ordine>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ordine>(entity =>
        {
            entity.HasKey(o => o.Id);
            entity.Property(o => o.Stato).HasConversion<string>().HasMaxLength(30).IsRequired();
            entity.Property(o => o.MotivoAnnullamento).HasMaxLength(500);
            entity.OwnsMany(o => o.Items, item =>
            {
                item.WithOwner().HasForeignKey("OrdineId");
                item.Property<int>("Id");
                item.HasKey("Id");
            });
        });
    }
}
