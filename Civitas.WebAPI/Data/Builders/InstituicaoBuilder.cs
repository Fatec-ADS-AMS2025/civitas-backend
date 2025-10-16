using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class InstituicaoBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Instituicao>().HasKey(i => i.Id);
            modelBuilder.Entity<Instituicao>().Property(i => i.CNPJ).IsRequired().HasMaxLength(14);
            modelBuilder.Entity<Instituicao>().Property(i => i.Nome).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Instituicao>().Property(i => i.Logradouro).IsRequired().HasMaxLength(200);
            modelBuilder.Entity<Instituicao>().Property(i => i.Numero).IsRequired().HasMaxLength(4);
            modelBuilder.Entity<Instituicao>().Property(i => i.Bairro).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Instituicao>().Property(i => i.CEP).IsRequired().HasMaxLength(8);
            modelBuilder.Entity<Instituicao>().Property(i => i.NomeRazaoSocial).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Instituicao>().Property(i => i.Email).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Instituicao>().Property(i => i.Telefone).IsRequired().HasMaxLength(15);
            modelBuilder.Entity<Instituicao>().Property(i => i.Cidade).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Instituicao>().Property(i => i.Estado).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Instituicao>().Property(i => i.Situacao).IsRequired();
            modelBuilder.Entity<Instituicao>()
                .HasOne<TipoInstituicao>(i => i.TipoInstituicao)
                .WithMany(ti => ti.Instituicoes)
                .HasForeignKey(i => i.IdTipoInstituicao)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
