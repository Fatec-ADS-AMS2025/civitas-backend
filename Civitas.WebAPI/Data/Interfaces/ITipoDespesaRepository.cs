using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface ITipoDespesaRepository : IGenericRepository<TipoDespesa>
    {
        Task<bool> ExisteUnidadesDeMedidaAtivas(int idTipoDespesa);
        Task<bool> ExistsByDescricaoNormalized(string descricaoNormalizada, int? ignoreId = null);
        Task<bool> HasDespesasVinculadas(int idTipoDespesa);
    }
}
