using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class TipoDespesaBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoDespesa>().HasKey(t => t.Id);
            modelBuilder.Entity<TipoDespesa>().Property(t => t.Descricao).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<TipoDespesa>().Property(t => t.SolicitaUc).IsRequired();
            modelBuilder.Entity<TipoDespesa>().Property(t => t.Situacao).IsRequired();
            modelBuilder.Entity<TipoDespesa>()
                .HasOne<UnidadeMedida>(u => u.UnidadeMedida)
                .WithMany(t => t.TiposDespesas)
                .HasForeignKey(u => u.IdUnidadeMedida)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
