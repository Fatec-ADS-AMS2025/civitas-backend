using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Objects.Dtos.Entities;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IInstituicaoRepository : IGenericRepository<Instituicao>
    {
        Task<IEnumerable<Instituicao>> GetInstituicaoByName(string name);
        Task<InstituicaoGastosDTO?> GetGastosByInstituicaoIdAsync(int instituicaoId);
        Task<InstituicaoOrcamentoDisponivelDTO?> GetOrcamentoDisponivelByInstituicaoIdAsync(int instituicaoId);
    }
}
