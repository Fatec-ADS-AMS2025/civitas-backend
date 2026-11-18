namespace Civitas.WebAPI.Objects.Contracts
{
    /// <summary>
    /// Operações que o sistema registra na auditoria.
    /// </summary>
    /// <remarks>
    /// Os valores são gravados exatamente como definidos aqui (maiúsculas, sem espaços).
    /// LOGOUT não faz parte da lista porque a API não possui encerramento de sessão:
    /// o JWT expira sozinho e não há evento para auditar.
    /// </remarks>
    public static class AuditoriaOperacao
    {
        /// <summary>Inclusão de um registro.</summary>
        public const string INSERT = "INSERT";

        /// <summary>Alteração de um registro, incluindo a exclusão lógica (Excluido de false para true).</summary>
        public const string UPDATE = "UPDATE";

        /// <summary>Exclusão física de um registro.</summary>
        public const string DELETE = "DELETE";

        /// <summary>Autenticação bem-sucedida de um usuário.</summary>
        public const string LOGIN = "LOGIN";

        /// <summary>Lista completa das operações aceitas.</summary>
        public static readonly IReadOnlyList<string> Todas = [INSERT, UPDATE, DELETE, LOGIN];
    }
}
