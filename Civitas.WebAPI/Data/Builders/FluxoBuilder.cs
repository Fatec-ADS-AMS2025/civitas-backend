using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Builders
{
    public class FluxoBuilder
    {
        public static void Build(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Fluxo>().HasKey(u => u.IdFluxo);
            modelBuilder.Entity<Fluxo>().Property(u => u.ValorPago).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Fluxo>().Property(u => u.Consumo).IsRequired().HasMaxLength(100);
            modelBuilder.Entity<Fluxo>().Property(u => u.Status).IsRequired();
        }
    }
}
