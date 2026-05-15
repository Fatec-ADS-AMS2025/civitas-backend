using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IDespesaRepository : IGenericRepository<Despesa>
    {
        Task<IEnumerable<Despesa>> GetByNumeroDocumentoAsync(string numeroDocumento);
        Task<IEnumerable<Despesa>> GetByHashDocumentoAsync(string hashDocumento);
        Task<IEnumerable<Despesa>> GetByCodigoAsync(string codigo);
        Task<IEnumerable<Despesa>> GetByUnidadeConsumidoraAsync(int idUnidadeConsumidora);
        Task<IEnumerable<Despesa>> GetByUsuarioAsync(int idUsuario);
        Task<IEnumerable<Despesa>> GetByStatusAsync(Status status);
        Task<decimal> SumValorPrevistoByOrcamentoAsync(int idOrcamento, int? ignoreId = null);
        Task<bool> ExistsByHashDocumentoAsync(string hashDocumento, int? ignoreId = null);
    }
}
