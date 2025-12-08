using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gerenciamento de Orçamentos (Tetos de Gastos).
    /// </summary>
    /// <remarks>
    /// Extende <see cref="IGenericService{Orcamento, OrcamentoDTO}"/> para incluir validações de integridade financeira.
    /// É o ponto de injeção para regras de negócio referentes ao planejamento de verbas.
    /// </remarks>
    public interface IOrcamentoService : IGenericService<Orcamento, OrcamentoDTO>
    {
        /// <summary>
        /// Verifica a existência de despesas vinculadas a um orçamento específico.
        /// </summary>
        /// <param name="idOrcamento">O identificador do orçamento a ser checado.</param>
        /// <returns>
        /// Uma tarefa contendo <c>true</c> se houver despesas utilizando este orçamento, ou <c>false</c> se estiver livre.
        /// </returns>
        /// <remarks>
        /// Contrato de Integridade:
        /// Deve ser invocado antes de permitir a exclusão de um orçamento. Se retornar verdadeiro, a interface do usuário deve bloquear a ação de deletar.
        /// </remarks>
        Task<bool> ExisteDespesaVinculada(int idOrcamento);
    }
}