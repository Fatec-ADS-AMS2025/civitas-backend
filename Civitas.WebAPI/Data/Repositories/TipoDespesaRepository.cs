using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class TipoDespesaRepository : GenericRepository<TipoDespesa>, ITipoDespesaRepository
    {
        private readonly AppDbContext _context;
        public TipoDespesaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExisteUnidadesDeMedidaAtivas(int idTipoDespesa)
        {
            return await _context.TiposDespesa
                .AsNoTracking()
                .AnyAsync(t => t.Id == idTipoDespesa && t.UnidadeMedida.Situacao == Situacao.ATIVO);
        }

        public async Task<bool> ExistsByDescricaoNormalized(string descricaoNormalizada, int? ignoreId = null)
        {
            var query = _context.TiposDespesa
                .AsNoTracking()
                .Where(t => t.Descricao.Trim().ToUpper() == descricaoNormalizada);

            if (ignoreId.HasValue)
            {
                query = query.Where(t => t.Id != ignoreId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasDespesasVinculadas(int idTipoDespesa)
        {
            return await _context.Despesas
                .AsNoTracking()
                .AnyAsync(d => d.UnidadeConsumidora.IdTipoDespesa == idTipoDespesa);
        }
    }
}
