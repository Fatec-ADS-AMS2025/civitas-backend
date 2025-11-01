using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    public interface ITipoDespesaService : IGenericService<TipoDespesa, TipoDespesaDTO>
    {
        Task<bool> ExisteUnidadesDeMedidaAtivas(int idTipoDespesa);
    }
}
