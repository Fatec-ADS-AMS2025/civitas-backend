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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            UsuarioBuilder.Build(modelBuilder);
        }
    }
}
