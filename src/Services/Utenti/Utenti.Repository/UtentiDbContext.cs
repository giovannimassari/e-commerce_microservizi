using Microsoft.EntityFrameworkCore;
using Utenti.Repository.Entities;

namespace Utenti.Repository;

public class UtentiDbContext : DbContext
{
    public UtentiDbContext(DbContextOptions<UtentiDbContext> options) : base(options)
    {
    }

    public DbSet<Utente> Utenti => Set<Utente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Utente>(entity =>
        {
            entity.ToTable("utenti");

            entity.HasKey(u => u.Id);

            entity.Property(u => u.Nome)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Cognome)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(u => u.Email)
                .IsUnique();

            entity.Property(u => u.PasswordHash)
                .IsRequired();

            entity.Property(u => u.Ruolo)
                .HasConversion<string>()    // salva il nome dell'enum invece che il valore (Salva "cliente", non 0)
                .HasMaxLength(50)
                .IsRequired();

            entity.Property(u => u.DataRegistrazione)
                .IsRequired();

            entity.Property(u => u.Attivo)
                .IsRequired()
                .HasDefaultValue(true);
        });

        base.OnModelCreating(modelBuilder);
    }
}