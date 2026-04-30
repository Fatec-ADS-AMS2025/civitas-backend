using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    public interface IUnidadeConsumidoraService : IGenericService<UnidadeConsumidora, UnidadeConsumidoraDTO>
    {
        Task<PaginatedResult<UnidadeConsumidoraDTO>> GetPageNaoExcluidos(PaginationQuery paginationQuery);
        Task<PaginatedResult<UnidadeConsumidoraDTO>> GetPageExcluidos(PaginationQuery paginationQuery);
        Task<UnidadeConsumidoraDTO?> GetByIdNaoExcluidoAsync(int id);
        Task<UnidadeConsumidoraDTO?> GetByIdentificadorNaoExcluidoAsync(string identificador);
        Task<IEnumerable<UnidadeConsumidoraDTO>> GetByInstituicaoNaoExcluidosAsync(int idInstituicao);
        Task<IEnumerable<UnidadeConsumidoraDTO>> GetBySecretariaNaoExcluidosAsync(int idSecretaria);
        Task<UnidadeConsumidoraDTO> ToggleStatusExclusaoAsync(int id);
    }
}
