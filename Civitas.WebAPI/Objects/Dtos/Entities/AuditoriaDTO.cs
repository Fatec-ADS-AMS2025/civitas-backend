namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência de dados para visualização e relatórios de Auditoria (Logs).
    /// </summary>
    /// <remarks>
    /// Usado somente na saída da API (telas de histórico de alterações). Os registros são gerados
    /// pelo backend; não existe endpoint que receba este DTO para criar, alterar ou excluir auditorias.
    /// Snapshots não trazem senhas nem tokens (gravados como "***").
    /// </remarks>
    public class AuditoriaDTO
    {
        /// <summary>Identificador único do registro de log.</summary>
        public int Id { get; set; }

        /// <summary>Data UTC da ocorrência, no formato yyyy-MM-dd.</summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>Horário UTC da ocorrência, no formato HH:mm:ss.</summary>
        public string Hora { get; set; } = string.Empty;

        /// <summary>Tipo de ação executada: INSERT, UPDATE, DELETE ou LOGIN.</summary>
        public string Operacao { get; set; } = string.Empty;

        /// <summary>Nome da entidade afetada, ou "Autenticacao" no LOGIN.</summary>
        public string NomeEntidade { get; set; } = string.Empty;

        /// <summary>Snapshot JSON dos dados ANTES da alteração. Vazio em INSERT e LOGIN.</summary>
        public string ValoresAtingidos { get; set; } = string.Empty;

        /// <summary>Snapshot JSON dos dados DEPOIS da alteração. Vazio em DELETE e LOGIN.</summary>
        public string NovosValores { get; set; } = string.Empty;

        /// <summary>
        /// Identificador do Usuário que realizou a operação.
        /// Nulo quando a operação ocorreu sem usuário autenticado.
        /// </summary>
        public int? UsuarioId { get; set; }
    }
}
