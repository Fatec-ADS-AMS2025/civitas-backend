using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IAuditoriaRepository : IGenericRepository<Auditoria>
    {
        Task<IEnumerable<Auditoria>> GetByUsuarioId(int usuarioId);
        Task<IEnumerable<Auditoria>> GetByEntidade(string nomeEntidade);
        Task<IEnumerable<Auditoria>> GetByOperacao(string operacao);
    }
}
