using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Validation;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class AuditoriaBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auditoria>().HasKey(a => a.Id);

            // O Id é sempre gerado pelo banco (identity).
            modelBuilder.Entity<Auditoria>()
                .Property(a => a.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.Data)
                .IsRequired()
                .HasMaxLength(10);

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.Hora)
                .IsRequired()
                .HasMaxLength(8);

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.Operacao)
                .IsRequired()
                .HasMaxLength(AuditoriaValidator.TamanhoMaximoOperacao);

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.NomeEntidade)
                .IsRequired()
                .HasMaxLength(AuditoriaValidator.TamanhoMaximoNomeEntidade);

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.ValoresAtingidos)
                .HasMaxLength(AuditoriaValidator.TamanhoMaximoSnapshot);

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.NovosValores)
                .HasMaxLength(AuditoriaValidator.TamanhoMaximoSnapshot);

            // Nulo representa operação sem usuário autenticado.
            modelBuilder.Entity<Auditoria>()
                .Property(a => a.UsuarioId)
                .IsRequired(false);

            // Excluir um usuário nunca remove o histórico dele.
            modelBuilder.Entity<Auditoria>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
