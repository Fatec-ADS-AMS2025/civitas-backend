using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Dtos.Entities;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de Auditoria (Logs do Sistema).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Prover métodos de consulta para rastreabilidade de eventos.
    /// - Registrar eventos de autenticação.
    /// Não expõe criação genérica, alteração, exclusão nem ativação/inativação: os registros
    /// de auditoria são gerados pelo backend e são imutáveis.
    /// Entradas inválidas geram <c>AuditoriaValidationException</c>.
    /// </remarks>
    public interface IAuditoriaService
    {
        /// <summary>Lista paginada de auditorias.</summary>
        Task<PaginatedResult<AuditoriaDTO>> GetPage(PaginationQuery paginationQuery);

        /// <summary>Busca uma auditoria pelo identificador (maior que zero). Retorna nulo se não existir.</summary>
        Task<AuditoriaDTO?> GetById(int id);

        /// <summary>
        /// Recupera o histórico de ações realizadas por um usuário específico.
        /// </summary>
        /// <param name="usuarioId">O identificador do usuário (autor da ação), maior que zero.</param>
        Task<IEnumerable<AuditoriaDTO>> GetByUsuarioId(int usuarioId);

        /// <summary>
        /// Recupera o histórico de alterações de uma entidade auditável.
        /// </summary>
        /// <param name="nomeEntidade">Nome da entidade (ex: "Despesa", "Fornecedor"), sem diferenciar maiúsculas.</param>
        Task<IEnumerable<AuditoriaDTO>> GetByEntidade(string? nomeEntidade);

        /// <summary>
        /// Recupera o histórico filtrado pelo tipo de operação.
        /// </summary>
        /// <param name="operacao">INSERT, UPDATE, DELETE ou LOGIN, sem diferenciar maiúsculas.</param>
        Task<IEnumerable<AuditoriaDTO>> GetByOperacao(string? operacao);

        /// <summary>
        /// Registra o LOGIN bem-sucedido de um usuário, com data e hora geradas pelo backend.
        /// </summary>
        Task RegistrarLoginAsync(int usuarioId);
    }
}
