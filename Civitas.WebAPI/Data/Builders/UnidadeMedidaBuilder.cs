using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class UnidadeMedidaBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnidadeMedida>().HasKey(u => u.Id);
            modelBuilder.Entity<UnidadeMedida>().Property(d => d.Descricao).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<UnidadeMedida>().Property(d => d.Abreviatura).IsRequired().HasMaxLength(45);
            modelBuilder.Entity<UnidadeMedida>().Property(d => d.Situacao).IsRequired();
            modelBuilder.Entity<UnidadeMedida>()
                .HasMany<TipoDespesa>(t => t.TiposDespesas)
                .WithOne(u => u.UnidadeMedida)
                .HasForeignKey(u => u.IdUnidadeMedida)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
