using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class UnidadeConsumidoraBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UnidadeConsumidora>().HasKey(uc => uc.Id);

            modelBuilder.Entity<UnidadeConsumidora>()
                .Property(uc => uc.Identificador)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<UnidadeConsumidora>()
                .HasIndex(uc => uc.Identificador)
                .IsUnique();

            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.IdInstituicao).IsRequired();
            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.IdTipoDespesa).IsRequired();
            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.IdSecretaria).IsRequired();
            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.IdOrcamento).IsRequired();
            modelBuilder.Entity<UnidadeConsumidora>().Property(uc => uc.IdFornecedor).IsRequired();

            modelBuilder.Entity<UnidadeConsumidora>()
                .Property(uc => uc.Excluido)
                .IsRequired()
                .HasDefaultValue(false);

            modelBuilder.Entity<UnidadeConsumidora>()
                .Property(uc => uc.DataExclusao)
                .IsRequired(false);

            modelBuilder.Entity<UnidadeConsumidora>()
                .HasOne(uc => uc.Instituicao)
                .WithMany()
                .HasForeignKey(uc => uc.IdInstituicao)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnidadeConsumidora>()
                .HasOne(uc => uc.TipoDespesa)
                .WithMany()
                .HasForeignKey(uc => uc.IdTipoDespesa)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnidadeConsumidora>()
                .HasOne(uc => uc.Secretaria)
                .WithMany()
                .HasForeignKey(uc => uc.IdSecretaria)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnidadeConsumidora>()
                .HasOne(uc => uc.Orcamento)
                .WithMany()
                .HasForeignKey(uc => uc.IdOrcamento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UnidadeConsumidora>()
                .HasOne(uc => uc.Fornecedor)
                .WithMany()
                .HasForeignKey(uc => uc.IdFornecedor)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
