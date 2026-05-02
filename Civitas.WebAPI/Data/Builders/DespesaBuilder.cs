using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class DespesaBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Despesa>().HasKey(d => d.Id);

            modelBuilder.Entity<Despesa>().Property(d => d.NumeroDocumento)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Despesa>().Property(d => d.Codigo)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Despesa>().Property(d => d.UC)
                .HasMaxLength(100);

            modelBuilder.Entity<Despesa>().Property(d => d.DataEmissao)
                .IsRequired()
                .HasMaxLength(20);

            modelBuilder.Entity<Despesa>().Property(d => d.ConsumoPrevisto)
                .IsRequired()
                .HasPrecision(18, 2);

            modelBuilder.Entity<Despesa>().Property(d => d.DataVencimento)
                .IsRequired();

            modelBuilder.Entity<Despesa>().Property(d => d.Status)
                .IsRequired();

            modelBuilder.Entity<Despesa>()
                .HasOne(d => d.TipoDespesa)
                .WithMany(td => td.Despesas)
                .HasForeignKey(d => d.IdTipoDespesa)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Despesa>()
                .HasOne(d => d.Orcamento)
                .WithMany(o => o.Despesas)
                .HasForeignKey(d => d.IdOrcamento)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Despesa>()
                .HasOne(d => d.Fornecedor)
                .WithMany(f => f.Despesas)
                .HasForeignKey(d => d.IdFornecedor)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Despesa>()
                .HasOne(d => d.Usuario)
                .WithMany(u => u.Despesas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Despesa>()
                .HasOne(d => d.Instituicao)
                .WithMany(i => i.Despesas)
                .HasForeignKey(d => d.IdInstituicao)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
