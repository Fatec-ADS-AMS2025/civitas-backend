using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IDespesaRepository : IGenericRepository<Despesa>
    {
        Task<bool> ExistsByNumeroDocumentoAndFornecedorAsync(
            string numeroDocumento,
            int idFornecedor,
            int? ignoreId = null);

        Task<decimal> SumConsumoByOrcamentoAsync(int idOrcamento, int? ignoreId = null);
    }
}
