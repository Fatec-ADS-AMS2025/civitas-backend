using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o servi�o de gerenciamento de Or�amentos (Tetos de Gastos).
    /// </summary>
    /// <remarks>
    /// Extende <see cref="IGenericService{Orcamento, OrcamentoDTO}"/> para incluir valida��es de integridade financeira.
    /// � o ponto de inje��o para regras de neg�cio referentes ao planejamento de verbas.
    /// </remarks>
    public interface IOrcamentoService : IGenericService<Orcamento, OrcamentoDTO>
    {
        /// <summary>
        /// Verifica a exist�ncia de despesas vinculadas a um or�amento espec�fico.
        /// </summary>
        /// <param name="idOrcamento">O identificador do or�amento a ser checado.</param>
        /// <returns>
        /// Uma tarefa contendo <c>true</c> se houver despesas utilizando este or�amento, ou <c>false</c> se estiver livre.
        /// </returns>
        /// <remarks>
        /// Contrato de Integridade:
        /// Deve ser invocado antes de permitir a exclus�o de um or�amento. Se retornar verdadeiro, a interface do usu�rio deve bloquear a a��o de deletar.
        /// </remarks>
        Task<bool> ExisteDespesaVinculada(int idOrcamento);

        Task RemoverAsync(int id);
    }
}