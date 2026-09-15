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
            entity.OwnsMany(o => o.Items, item =>
            {
                item.WithOwner().HasForeignKey("OrdineId");
                item.Property<int>("Id");
                item.HasKey("Id");
            });
        });
    }
}