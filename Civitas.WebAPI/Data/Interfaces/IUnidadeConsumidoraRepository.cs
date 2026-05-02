using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IUnidadeConsumidoraRepository : IGenericRepository<UnidadeConsumidora>
    {
        Task<PaginatedResult<UnidadeConsumidora>> GetPageNaoExcluidos(PaginationQuery paginationQuery);
        Task<PaginatedResult<UnidadeConsumidora>> GetPageExcluidos(PaginationQuery paginationQuery);
        Task<UnidadeConsumidora?> GetByIdNaoExcluidoAsync(int id);
        Task<UnidadeConsumidora?> GetByIdentificadorNaoExcluidoAsync(string identificador);
        Task<IEnumerable<UnidadeConsumidora>> GetByInstituicaoNaoExcluidosAsync(int idInstituicao);
        Task<IEnumerable<UnidadeConsumidora>> GetBySecretariaNaoExcluidosAsync(int idSecretaria);
        Task<bool> ExistsByIdentificadorAsync(string identificador, int? ignoreId = null);
        Task<bool> InstituicaoExistsAsync(int idInstituicao);
        Task<bool> TipoDespesaExistsAsync(int idTipoDespesa);
        Task<bool> SecretariaExistsAsync(int idSecretaria);
        Task<bool> OrcamentoExistsAsync(int idOrcamento);
        Task<bool> FornecedorExistsAsync(int idFornecedor);
    }
}
