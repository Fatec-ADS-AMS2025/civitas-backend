using Civitas.WebAPI.Data.Builders;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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
        public DbSet<Documento> Documento { get; set; }
        public DbSet<Auditoria> Auditorias { get; set; }
        public DbSet<TipoInstituicao> TipoInstituicoes { get; set; }
        public DbSet<Instituicao> Instituicoes { get; set; }
        public DbSet<Fluxo> Fluxos { get; set; }
        public DbSet<UnidadeMedida> UnidadesMedida { get; set; }
        public DbSet<TipoDespesa> TiposDespesa { get; set; }
        public DbSet<Orcamento> Orcamentos { get; set; }
        public DbSet<Despesa> Despesas { get; set; }
        public DbSet<TipoCodigo> TipoCodigos { get; set; }
        public DbSet<UnidadeConsumidora> UnidadesConsumidoras { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            UsuarioBuilder.Build(modelBuilder);
            FornecedorBuilder.Build(modelBuilder);
            SecretariaBuilder.Build(modelBuilder);
            DocumentoBuilder.Build(modelBuilder);
            AuditoriaBuilder.Build(modelBuilder);
            TipoInstituicaoBuilder.Build(modelBuilder);
            InstituicaoBuilder.Build(modelBuilder);
            FluxoBuilder.Build(modelBuilder);
            UnidadeMedidaBuilder.Build(modelBuilder);
            TipoDespesaBuilder.Build(modelBuilder);
            OrcamentoBuilder.Build(modelBuilder);
            UnidadeConsumidoraBuilder.Build(modelBuilder);
            DespesaBuilder.Build(modelBuilder);
        }
    }
}
