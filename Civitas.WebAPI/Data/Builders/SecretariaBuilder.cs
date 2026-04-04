using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class SecretariaBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Secretaria>().HasKey(s => s.IdSecretaria);
            modelBuilder.Entity<Secretaria>().Property(s => s.Situacao).IsRequired();
            modelBuilder.Entity<Secretaria>().Property(s => s.Descricao).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<Secretaria>().Property(s => s.Cnpj).IsRequired().HasMaxLength(14);
            modelBuilder.Entity<Secretaria>().Property(s => s.Nome).IsRequired().HasMaxLength(150);
            modelBuilder.Entity<Secretaria>().Property(s => s.Logradouro).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Secretaria>().Property(s => s.Numero).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Secretaria>().Property(s => s.Bairro).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Secretaria>().Property(s => s.Cep).IsRequired().HasMaxLength(8);
            modelBuilder.Entity<Secretaria>().Property(s => s.NomeRazaoSocial).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Secretaria>().Property(s => s.Email).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<Secretaria>().Property(s => s.Telefone).IsRequired().HasMaxLength(11);
            modelBuilder.Entity<Secretaria>().Property(s => s.Cidade).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Secretaria>().Property(s => s.Estado).IsRequired().HasMaxLength(2);

        }
    }
}
