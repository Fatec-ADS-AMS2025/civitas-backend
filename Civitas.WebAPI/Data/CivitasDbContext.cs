using Microsoft.EntityFrameworkCore;
using Civitas.WebAPI.Models;

namespace Civitas.WebAPI.Data;

public class CivitasDbContext : DbContext
{
    public CivitasDbContext(DbContextOptions<CivitasDbContext> options) : base(options)
    {
    }

    public DbSet<Secretaria> Secretarias { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Secretaria>(entity =>
        {
            entity.HasKey(e => e.IdSecretaria);
            entity.Property(e => e.IdSecretaria).ValueGeneratedOnAdd();
            entity.Property(e => e.DataCriacao).HasDefaultValueSql("NOW()");
            entity.HasIndex(e => e.Cnpj).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}