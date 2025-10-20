using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;


namespace Civitas.WebAPI.Data.Builders
{
    public class DocumentoBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Documento>().HasKey(d => d.IdDocumentos);
            modelBuilder.Entity<Documento>().Property(d => d.Digitalizacao).IsRequired();
            modelBuilder.Entity<Documento>().Property(d => d.NumeroDocumento).IsRequired();
        }
    }
}
