using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gerenciamento de Secretarias (Órgãos Gestores).
    /// </summary>
    /// <remarks>
    /// Herda as operações padrão de <see cref="IGenericService{Secretaria, SecretariaDTO}"/>.
    /// Atua como contrato para a injeção de dependência das regras de negócio que envolvem a hierarquia administrativa superior
    /// e a validação de dados de órgãos públicos (CNPJ, Endereço).
    /// </remarks>
    public interface ISecretariaService : IGenericService<Secretaria, SecretariaDTO>
    {
    Task ValidarCadastroAsync(SecretariaDTO entityDTO, int? id = null);
    }
}