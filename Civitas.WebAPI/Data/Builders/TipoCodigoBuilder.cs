using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class TipoCodigoBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TipoCodigo>().HasKey(f => f.Id);
            modelBuilder.Entity<TipoCodigo>().Property(f => f.Nome).IsRequired();
            modelBuilder.Entity<TipoCodigo>().Property(f => f.Descricao).IsRequired();
        }
    }
}
