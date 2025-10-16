using Civitas.WebAPI.Data.Builders;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;
using System.Runtime.ConstrainedExecution;
using System.Text.RegularExpressions;

namespace Civitas.WebAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Fornecedor> Fornecedores { get; set; }
        public DbSet<Secretaria> Secretarias { get; set; }
        public DbSet<Secretaria> Documentos { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            UsuarioBuilder.Build(modelBuilder);
            FornecedorBuilder.Build(modelBuilder);
            SecretariaBuilder.Build(modelBuilder);
            DocumentosBuilder.Build(modelBuilder);
        }
    }
}
