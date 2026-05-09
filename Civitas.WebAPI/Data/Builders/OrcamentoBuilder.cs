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

            modelBuilder.Entity<Orcamento>().Property(o => o.IdInstituicao)
                .IsRequired()
                .HasColumnName("idinstituicao");

            modelBuilder.Entity<Orcamento>().Property(o => o.IdTipoDespesa)
                .IsRequired()
                .HasColumnName("idtipodespesa");

            modelBuilder.Entity<Orcamento>().Property(o => o.JaneiroQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.JaneiroValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.FevereiroQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.FevereiroValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.MarcoQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.MarcoValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.AbrilQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.AbrilValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.MaioQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.MaioValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.JunhoQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.JunhoValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.JulhoQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.JulhoValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.AgostoQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.AgostoValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.SetembroQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.SetembroValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.OutubroQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.OutubroValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.NovembroQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.NovembroValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>().Property(o => o.DezembroQuantidadeConsumo).HasPrecision(18, 2);
            modelBuilder.Entity<Orcamento>().Property(o => o.DezembroValorConsumo).HasPrecision(18, 2);

            modelBuilder.Entity<Orcamento>()
                .HasOne<Instituicao>(o => o.Instituicao)
                .WithMany(i => i.Orcamento)
                .HasForeignKey(o => o.IdInstituicao)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Orcamento>()
                .HasOne<TipoDespesa>(o => o.TipoDespesa)
                .WithMany(td => td.Orcamento)
                .HasForeignKey(o => o.IdTipoDespesa)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}