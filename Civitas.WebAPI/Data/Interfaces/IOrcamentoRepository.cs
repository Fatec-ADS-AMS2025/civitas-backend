using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IOrcamentoRepository : IGenericRepository<Orcamento>
    {
        Task<bool> ExisteDespesaVinculada(int idOrcamento);

        Task<decimal> SumValorPrevistoByOrcamentoAsync(int idOrcamento);
    }
}
