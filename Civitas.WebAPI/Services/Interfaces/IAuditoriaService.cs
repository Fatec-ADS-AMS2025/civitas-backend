using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de Auditoria (Logs do Sistema).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Prover métodos especializados de busca para rastreabilidade de eventos.
    /// - Permitir filtrar o histórico de alterações por Autor, Alvo ou Tipo de Ação.
    /// </remarks>
    public interface IAuditoriaService : IGenericService<Auditoria, AuditoriaDTO>
    {
        /// <summary>
        /// Recupera o histórico de ações realizadas por um usuário específico.
        /// </summary>
        /// <param name="usuarioId">O identificador único do usuário (autor da ação).</param>
        /// <returns>Uma coleção de logs de auditoria vinculados a este usuário.</returns>
        /// <remarks>
        /// Utilidade: Investigar o comportamento de um funcionário ou rastrear erros operacionais cometidos por um login específico.
        /// </remarks>
        Task<IEnumerable<AuditoriaDTO>> GetByUsuarioId(int usuarioId);

        /// <summary>
        /// Recupera o histórico de alterações sofridas por uma entidade específica (Tabela).
        /// </summary>
        /// <param name="nomeEntidade">O nome da entidade/tabela (ex: "Despesa", "Fornecedor").</param>
        /// <returns>Uma coleção de logs referentes à entidade solicitada.</returns>
        /// <remarks>
        /// Utilidade: Visualizar a "linha do tempo" de alterações de um módulo. 
        /// Ex: "Quero ver todas as modificações feitas em Orçamentos".
        /// </remarks>
        Task<IEnumerable<AuditoriaDTO>> GetByEntidade(string nomeEntidade);

        /// <summary>
        /// Recupera o histórico filtrado pelo tipo de operação executada no banco de dados.
        /// </summary>
        /// <param name="operacao">O tipo de operação (ex: "INSERT", "UPDATE", "DELETE").</param>
        /// <returns>Uma coleção de logs que correspondem à operação.</returns>
        /// <remarks>
        /// Utilidade: Auditoria de segurança.
        /// Ex: Filtrar apenas por "DELETE" para verificar o que foi excluído do sistema recentemente.
        /// </remarks>
        Task<IEnumerable<AuditoriaDTO>> GetByOperacao(string operacao);
    }
}