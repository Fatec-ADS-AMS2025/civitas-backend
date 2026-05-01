using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class UnidadeConsumidoraBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnidadeConsumidora>().HasKey(uc => uc.Id);

            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.Codigo)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.Situacao)
                .IsRequired();

            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.IdTipoDespesa)
                .IsRequired()
                .HasColumnName("idtipodespesa");

            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.IdOrcamento)
                .IsRequired()
                .HasColumnName("idorcamento");

            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.IdInstituicao)
                .IsRequired()
                .HasColumnName("idinstituicao");

            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.IdFornecedor)
                .IsRequired()
                .HasColumnName("idfornecedor");

            modelBuilder.Entity<UnidadeConsumidora>()
                .HasOne(uc => uc.TipoDespesa)
                .WithMany(td => td.UnidadesConsumidoras)
                .HasForeignKey(uc => uc.IdTipoDespesa)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnidadeConsumidora>()
                .HasOne(uc => uc.Orcamento)
                .WithMany(o => o.UnidadesConsumidoras)
                .HasForeignKey(uc => uc.IdOrcamento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnidadeConsumidora>()
                .HasOne(uc => uc.Instituicao)
                .WithMany(i => i.UnidadesConsumidoras)
                .HasForeignKey(uc => uc.IdInstituicao)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnidadeConsumidora>()
                .HasOne(uc => uc.Fornecedor)
                .WithMany(f => f.UnidadesConsumidoras)
                .HasForeignKey(uc => uc.IdFornecedor)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
