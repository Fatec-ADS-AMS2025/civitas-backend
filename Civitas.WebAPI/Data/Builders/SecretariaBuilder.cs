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
            modelBuilder.Entity<Secretaria>().Property(s => s.Descricao).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Secretaria>().Property(s => s.Cnpj).IsRequired().HasMaxLength(18);
            modelBuilder.Entity<Secretaria>().Property(s => s.Nome).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Secretaria>().Property(s => s.Logradouro).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Secretaria>().Property(s => s.Numero).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Secretaria>().Property(s => s.Bairro).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Secretaria>().Property(s => s.Cep).IsRequired().HasMaxLength(10);
            modelBuilder.Entity<Secretaria>().Property(s => s.NomeRazaoSocial).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Secretaria>().Property(s => s.Email).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Secretaria>().Property(s => s.Telefone).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Secretaria>().Property(s => s.Cidade).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Secretaria>().Property(s => s.Estado).IsRequired().HasMaxLength(2);

        }
    }
}
