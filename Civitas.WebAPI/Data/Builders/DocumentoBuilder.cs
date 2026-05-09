using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;


namespace Civitas.WebAPI.Data.Builders
{
    public class DocumentoBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Documento>().HasKey(d => d.IdDocumento);
            modelBuilder.Entity<Documento>().Property(d => d.Digitalizacao).IsRequired();
            modelBuilder.Entity<Documento>().Property(d => d.NumeroDocumento).IsRequired();
            modelBuilder.Entity<Documento>()
                .HasOne<Fornecedor>(i => i.Fornecedor)
                .WithMany(ti => ti.Documentos)
                .HasForeignKey(i => i.IdFornecedor)
                .OnDelete(DeleteBehavior.Restrict);
            
        }
    }
}
