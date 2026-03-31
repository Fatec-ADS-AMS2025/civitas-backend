using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gerenciamento de Fornecedores (Credores).
    /// </summary>
    /// <remarks>
    /// Herda as operações de <see cref="IGenericService{Fornecedor, FornecedorDTO}"/>.
    /// Utilizada via injeção de dependência para aplicar regras de negócio sobre as empresas prestadoras de serviço,
    /// garantindo que apenas fornecedores válidos sejam vinculados às despesas.
    /// </remarks>
    public interface IFornecedorService : IGenericService<Fornecedor, FornecedorDTO>
    {
        Task ValidarCadastroAsync(FornecedorDTO entityDTO, int? id = null);
    }
}
