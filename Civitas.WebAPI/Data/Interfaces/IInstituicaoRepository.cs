using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IInstituicaoRepository : IGenericRepository<Instituicao>
    {
        Task<IEnumerable<Instituicao>> GetInstituicaoByName(string name);
    }
}
