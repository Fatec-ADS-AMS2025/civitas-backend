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

            modelBuilder.Entity<Despesa>().Property(d => d.DataEmissao)
                .IsRequired();

            modelBuilder.Entity<Despesa>().Property(d => d.ValorPrevisto)
                .IsRequired()
                .HasPrecision(18, 2);

            modelBuilder.Entity<Despesa>().Property(d => d.ValorPago)
                .IsRequired()
                .HasPrecision(18, 2);

            modelBuilder.Entity<Despesa>().Property(d => d.ConsumoPrevisto)
                .IsRequired()
                .HasPrecision(18, 2);

            modelBuilder.Entity<Despesa>().Property(d => d.ConsumoReal)
                .IsRequired()
                .HasPrecision(18, 2);

            modelBuilder.Entity<Despesa>().Property(d => d.DataVencimento)
                .IsRequired();

            modelBuilder.Entity<Despesa>().Property(d => d.DataPagamento)
                .IsRequired(false);

            modelBuilder.Entity<Despesa>().Property(d => d.Status)
                .IsRequired();

            modelBuilder.Entity<Despesa>().Property(d => d.IdUsuario)
                .IsRequired()
                .HasColumnName("idusuario");

            modelBuilder.Entity<Despesa>().Property(d => d.IdUnidadeConsumidora)
                .IsRequired()
                .HasColumnName("idunidadeconsumidora");

            modelBuilder.Entity<Despesa>().Property(d => d.ValoresOpcionais)
                .HasColumnName("valoresopcionais")
                .IsRequired(false);

            modelBuilder.Entity<Despesa>()
                .HasOne(d => d.Usuario)
                .WithMany(u => u.Despesas)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Despesa>()
                .HasOne(d => d.UnidadeConsumidora)
                .WithMany(uc => uc.Despesas)
                .HasForeignKey(d => d.IdUnidadeConsumidora)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
