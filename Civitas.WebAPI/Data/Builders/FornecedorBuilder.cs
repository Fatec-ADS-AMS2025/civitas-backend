using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class FornecedorBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fornecedor>().HasKey(f => f.IdFornecedor);
            modelBuilder.Entity<Fornecedor>().Property(f => f.NomeFantasia).IsRequired().HasMaxLength(150);
            modelBuilder.Entity<Fornecedor>().Property(f => f.Situacao).IsRequired();
            modelBuilder.Entity<Fornecedor>().Property(f => f.Cnpj).IsRequired().HasMaxLength(14);
            modelBuilder.Entity<Fornecedor>().HasIndex(f => f.Cnpj).IsUnique();
            modelBuilder.Entity<Fornecedor>().Property(f => f.Nome).IsRequired().HasMaxLength(150);
            modelBuilder.Entity<Fornecedor>().Property(f => f.Logradouro).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Fornecedor>().Property(f => f.Numero).IsRequired().HasMaxLength(20);
            modelBuilder.Entity<Fornecedor>().Property(f => f.Bairro).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Fornecedor>().Property(f => f.Cep).IsRequired().HasMaxLength(8);
            modelBuilder.Entity<Fornecedor>().Property(f => f.Telefone).IsRequired().HasMaxLength(11);
            modelBuilder.Entity<Fornecedor>().Property(f => f.Email).IsRequired().HasMaxLength(150);
            modelBuilder.Entity<Fornecedor>().Property(f => f.Cidade).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Fornecedor>().Property(f => f.Estado).IsRequired().HasMaxLength(2);
            modelBuilder.Entity<Fornecedor>()
               .HasMany<Documento>(i => i.Documentos)
               .WithOne(ti => ti.Fornecedor)
               .HasForeignKey(i => i.IdFornecedor)
               .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Fornecedor>()
                .HasOne(f => f.TipoDespesa)
                .WithMany(td => td.Fornecedor)
                .HasForeignKey(f => f.IdTipoDespesa)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
