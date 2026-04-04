using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class UsuarioBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuario>().HasKey(u => u.Id);
            modelBuilder.Entity<Usuario>().Property(u => u.Cpf).IsRequired().HasMaxLength(11);
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Cpf).IsUnique();
            modelBuilder.Entity<Usuario>().Property(u => u.Nome).IsRequired().HasMaxLength(150);
            modelBuilder.Entity<Usuario>().Property(u => u.Rg).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Usuario>().Property(u => u.Logradouro).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Usuario>().Property(u => u.Numero).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Usuario>().Property(u => u.Cidade).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Usuario>().Property(u => u.Estado).IsRequired().HasMaxLength(2);
            modelBuilder.Entity<Usuario>().Property(u => u.Cep).IsRequired().HasMaxLength(8);
            modelBuilder.Entity<Usuario>().Property(u => u.Email).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<Usuario>().Property(u => u.Senha).IsRequired().HasMaxLength(256);
            modelBuilder.Entity<Usuario>().Property(u => u.Situacao).IsRequired();
            modelBuilder.Entity<Usuario>().Property(u => u.Matricula).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Usuario>().HasIndex(u => u.Matricula).IsUnique();
            modelBuilder.Entity<Usuario>().Property(u => u.TipoUsuario).IsRequired();
            modelBuilder.Entity<Usuario>().Property(u => u.Bairro).IsRequired().HasMaxLength(100);
        }
    }
}
