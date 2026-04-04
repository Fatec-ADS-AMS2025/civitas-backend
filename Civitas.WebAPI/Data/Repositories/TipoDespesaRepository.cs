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
            return await _context.UnidadesMedida
                .AnyAsync(i => i.Id == idTipoDespesa && i.Situacao == Situacao.ATIVO);
        }
    }
}
