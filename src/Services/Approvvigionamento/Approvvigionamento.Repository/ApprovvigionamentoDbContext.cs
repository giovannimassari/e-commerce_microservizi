using Approvvigionamento.Repository.Entities;
using Microsoft.EntityFrameworkCore;

namespace Approvvigionamento.Repository;

public class ApprovvigionamentoDbContext(DbContextOptions<ApprovvigionamentoDbContext> options) : DbContext(options)
{
    public DbSet<RichiestaRifornimento> Richieste => Set<RichiestaRifornimento>();
    public DbSet<Fornitura> Forniture => Set<Fornitura>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<RichiestaRifornimento>(entity =>
        {
            entity.ToTable("richieste_rifornimento");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Stato).HasConversion<string>().HasMaxLength(20).IsRequired();
            entity.HasIndex(r => new { r.ProductId, r.Stato });
        });

        modelBuilder.Entity<Fornitura>(entity =>
        {
            entity.ToTable("forniture");
            entity.HasKey(f => f.Id);
            entity.HasIndex(f => f.RichiestaRifornimentoId);
        });

        base.OnModelCreating(modelBuilder);
    }
}
