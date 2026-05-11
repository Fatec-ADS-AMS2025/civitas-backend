using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Objects.Dtos.Entities;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IInstituicaoRepository : IGenericRepository<Instituicao>
    {
        Task<IEnumerable<Instituicao>> GetInstituicaoByName(string name);
        Task<bool> ExistsByCnpjAsync(string cnpj, int? ignoreId = null);
        Task<bool> ExistsByEmailAsync(string email, int? ignoreId = null);
        Task<bool> HasDespesasPendentesAsync(int instituicaoId);
        Task<InstituicaoGastosDTO?> GetGastosByInstituicaoIdAsync(int instituicaoId, int tipoDespesaId);
        Task<InstituicaoOrcamentoDisponivelDTO?> GetOrcamentoDisponivelByInstituicaoIdAsync(int instituicaoId);
    }
}
