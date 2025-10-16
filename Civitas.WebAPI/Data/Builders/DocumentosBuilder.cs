using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;


namespace Civitas.WebAPI.Data.Builders
{
    public class DocumentosBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Documentos>().HasKey(d => d.IdDocumentos);
            modelBuilder.Entity<Documentos>().Property(d => d.Digitalizacao).IsRequired();
            modelBuilder.Entity<Documentos>().Property(d => d.NumeroDocumento).IsRequired();
        }
    }
}
