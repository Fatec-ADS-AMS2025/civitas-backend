using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    public interface IInstituicaoService : IGenericService<Instituicao, InstituicaoDTO>
    {
        Task<IEnumerable<InstituicaoDTO>> GetInstituicaoByName(string nome);
    }
}
