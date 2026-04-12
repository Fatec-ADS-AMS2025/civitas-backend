using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class OrcamentoBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Orcamento>().HasKey(o => o.IdOrcamento);
            modelBuilder.Entity<Orcamento>().Property(o => o.AnoOrcamento).IsRequired();
            modelBuilder.Entity<Orcamento>().Property(o => o.ValorOrcamento).IsRequired().HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.IdInstituicao).IsRequired().HasColumnName("idinstituicao"); ;

            // Relacionamento com Instituicao
            modelBuilder.Entity<Orcamento>()
                .HasOne<Instituicao>(o => o.Instituicao)
                .WithMany(i => i.Orcamento)
                .HasForeignKey(o => o.IdInstituicao)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
