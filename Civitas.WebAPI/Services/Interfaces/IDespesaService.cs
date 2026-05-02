using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gerenciamento de Despesas.
    /// </summary>
    public interface IDespesaService : IGenericService<Despesa, DespesaDTO>
    {
        Task ValidarCadastroAsync(DespesaDTO entityDTO, int? id = null);
        Task<IEnumerable<DespesaDTO>> GetByNumeroDocumentoAsync(string numeroDocumento);
        Task<IEnumerable<DespesaDTO>> GetByCodigoAsync(string codigo);
        Task<IEnumerable<DespesaDTO>> GetByUnidadeConsumidoraAsync(int idUnidadeConsumidora);
        Task<IEnumerable<DespesaDTO>> GetByUsuarioAsync(int idUsuario);
        Task<IEnumerable<DespesaDTO>> GetByStatusAsync(Status status);
        Task<DespesaDTO> AlterarStatusAsync(int id, Status novoStatus);
    }
}
