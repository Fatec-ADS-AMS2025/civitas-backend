using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    public interface ITipoInstituicaoService : IGenericService<TipoInstituicao, TipoInstituicaoDTO>
    {
        Task<bool> ExisteInstituicoesAtivas(int idTipoInstituicao);
    }
}
