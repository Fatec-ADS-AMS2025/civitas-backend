using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Civitas.WebAPI.Data.Repositories
{
    public class UnidadeMedidaRepository : GenericRepository<UnidadeMedida>, IUnidadeMedidaRepository
    {
        private readonly AppDbContext _context;
        public UnidadeMedidaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByDescricaoNormalized(string descricaoNormalizada, int? ignoreId = null)
        {
            var query = _context.UnidadesMedida.AsNoTracking().Where(u => !u.Excluido);
            if (ignoreId.HasValue) query = query.Where(u => u.Id != ignoreId.Value);

            return (await query.Select(u => u.Descricao).ToListAsync())
                .Any(descricao => NormalizeForComparison(descricao) == descricaoNormalizada);
        }

        public async Task<bool> ExistsByAbreviaturaNormalized(string abreviaturaNormalizada, int? ignoreId = null)
        {
            var query = _context.UnidadesMedida.AsNoTracking().Where(u => !u.Excluido);
            if (ignoreId.HasValue) query = query.Where(u => u.Id != ignoreId.Value);

            return (await query.Select(u => u.Abreviatura).ToListAsync())
                .Any(abreviatura => NormalizeForComparison(abreviatura) == abreviaturaNormalizada);
        }

        public Task<bool> HasTiposDespesaAtivosVinculados(int idUnidadeMedida)
        {
            return _context.TiposDespesa.AsNoTracking().AnyAsync(t =>
                t.IdUnidadeMedida == idUnidadeMedida &&
                t.Situacao == Situacao.ATIVO &&
                !t.Excluido);
        }

        private static string NormalizeForComparison(string value)
        {
            return Regex.Replace(value.Trim(), "\\s+", " ").ToUpperInvariant();
        }
    }
}
