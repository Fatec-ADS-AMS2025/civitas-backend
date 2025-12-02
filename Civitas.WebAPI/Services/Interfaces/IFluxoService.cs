using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gerenciamento de Fluxos (Execução Financeira e Pagamentos).
    /// </summary>
    /// <remarks>
    /// Herda as operações padrão de <see cref="IGenericService{Fluxo, FluxoDTO}"/>.
    /// Atua como contrato para a injeção de dependência das regras de baixa de pagamentos, controle de vencimentos e medição de consumo.
    /// </remarks>
    public interface IFluxoService : IGenericService<Fluxo, FluxoDTO>
    {
    }
}