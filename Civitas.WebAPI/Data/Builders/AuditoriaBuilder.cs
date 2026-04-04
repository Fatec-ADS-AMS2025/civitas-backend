using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class AuditoriaBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Auditoria>().HasKey(a => a.Id);
            
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
                .HasMaxLength(100);

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.NomeEntidade)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.ValoresAtingidos)
                .HasMaxLength(500);

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.NovosValores)
                .HasMaxLength(500);

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.Situacao)
                .IsRequired();

            modelBuilder.Entity<Auditoria>()
                .Property(a => a.UsuarioId)
                .IsRequired();

            // Configuração do relacionamento com Usuario
            modelBuilder.Entity<Auditoria>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
