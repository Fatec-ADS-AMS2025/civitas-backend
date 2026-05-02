using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gerenciamento de Despesas (Contas a Pagar).
    /// </summary>
    /// <remarks>
    /// Herda as operações padrão de CRUD (Create, Read, Update, Delete) através de <see cref="IGenericService{Despesa, DespesaDTO}"/>.
    /// Serve como ponto de injeção de dependência para a lógica de negócios financeira, permitindo desacoplamento entre a API e a implementação concreta.
    /// </remarks>
    public interface IDespesaService : IGenericService<Despesa, DespesaDTO>
    {
        Task ValidarCadastroAsync(DespesaDTO entityDTO, int? id = null);
    }
}
