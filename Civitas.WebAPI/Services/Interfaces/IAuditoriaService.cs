using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    public interface IAuditoriaService : IGenericService<Auditoria, AuditoriaDTO>
    {
        Task<IEnumerable<AuditoriaDTO>> GetByUsuarioId(int usuarioId);
        Task<IEnumerable<AuditoriaDTO>> GetByEntidade(string nomeEntidade);
        Task<IEnumerable<AuditoriaDTO>> GetByOperacao(string operacao);
    }
}
