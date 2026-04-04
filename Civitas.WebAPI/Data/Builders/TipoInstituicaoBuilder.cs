using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class TipoInstituicaoBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoInstituicao>().HasKey(i => i.Id);
            modelBuilder.Entity<TipoInstituicao>().Property(i => i.Descricao).IsRequired().HasMaxLength(500);
            modelBuilder.Entity<TipoInstituicao>()
                .HasMany<Instituicao>(i => i.Instituicoes)
                .WithOne(ti => ti.TipoInstituicao)
                .HasForeignKey(i => i.IdTipoInstituicao)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
