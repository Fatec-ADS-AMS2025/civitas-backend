using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class OrcamentoRepository : GenericRepository<Orcamento>, IOrcamentoRepository
    {
        private readonly AppDbContext _context;

        public OrcamentoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExisteDespesaVinculada(int idOrcamento)
        {
            return await _context.Despesas
                .AnyAsync(d => d.IdOrcamento == idOrcamento);
        }

        public async Task<decimal> SumConsumoByOrcamentoAsync(int idOrcamento)
        {
            var total = await _context.Despesas
                .AsNoTracking()
                .Where(despesa => despesa.IdOrcamento == idOrcamento)
                .SumAsync(despesa => (decimal?)despesa.ConsumoPrevisto) ?? 0m;

            return Math.Round(total, 2, MidpointRounding.AwayFromZero);
        }
    }
}
