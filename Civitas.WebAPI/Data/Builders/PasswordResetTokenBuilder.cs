using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public static class PasswordResetTokenBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PasswordResetToken>().HasKey(token => token.Id);
            modelBuilder.Entity<PasswordResetToken>().Property(token => token.TokenHash).IsRequired().HasMaxLength(64);
            modelBuilder.Entity<PasswordResetToken>().Property(token => token.CriadoEm).IsRequired();
            modelBuilder.Entity<PasswordResetToken>().Property(token => token.ExpiraEm).IsRequired();
            modelBuilder.Entity<PasswordResetToken>()
                .HasOne(token => token.Usuario)
                .WithMany()
                .HasForeignKey(token => token.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<PasswordResetToken>().HasIndex(token => token.TokenHash).IsUnique();
            modelBuilder.Entity<PasswordResetToken>()
                .HasIndex(token => token.UsuarioId)
                .HasFilter("utilizado_em IS NULL")
                .IsUnique();
        }
    }
}
