using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Entidade responsável pelo registro histórico de operações (Logs) realizadas no sistema.
    /// Mapeia a tabela 'auditoria' do banco de dados.
    /// </summary>
    /// <remarks>
    /// O registro de auditoria é imutável e essencial para rastreabilidade de segurança.
    /// Ele captura quem realizou a ação, qual entidade foi afetada e o estado dos dados antes e depois da operação.
    /// Os registros são gerados pelo backend (AppDbContext e AuthService) e validados pelo AuditoriaValidator;
    /// nenhum cliente da API cria, altera ou exclui auditorias.
    /// </remarks>
    [Table("auditoria")]
    public class Auditoria
    {
        /// <summary>
        /// Identificador único do registro de log (Chave Primária), gerado pelo banco de dados.
        /// </summary>
        /// <remarks>Não pode ser definido nem alterado pela aplicação.</remarks>
        [Column("id")]
        public int Id { get; private set; }

        /// <summary>
        /// Data UTC em que a operação ocorreu, no formato exato yyyy-MM-dd.
        /// </summary>
        /// <example>2026-11-07</example>
        [Column("data")]
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Horário UTC em que a operação ocorreu, no formato 24 horas HH:mm:ss.
        /// </summary>
        /// <remarks>Gerado a partir do mesmo instante usado em <see cref="Data"/>.</remarks>
        /// <example>14:30:59</example>
        [Column("hora")]
        public string Hora { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de operação realizada.
        /// </summary>
        /// <example>INSERT, UPDATE, DELETE, LOGIN.</example>
        [Column("operacao")]
        public string Operacao { get; set; } = string.Empty;

        /// <summary>
        /// Nome da entidade que sofreu a alteração, ou "Autenticacao" nos eventos de LOGIN.
        /// </summary>
        /// <example>Usuario, Despesa, Fornecedor.</example>
        [Column("nome_entidade")]
        public string NomeEntidade { get; set; } = string.Empty;

        /// <summary>
        /// Snapshot JSON dos dados ANTES da alteração (Valores Antigos).
        /// </summary>
        /// <remarks>
        /// UPDATE: campos alterados com os valores anteriores. DELETE: objeto removido.
        /// INSERT e LOGIN: vazio. Dados sensíveis são gravados como "***".
        /// </remarks>
        [Column("valores_atingidos")]
        public string ValoresAtingidos { get; set; } = string.Empty;

        /// <summary>
        /// Snapshot JSON dos dados DEPOIS da alteração (Novos Valores).
        /// </summary>
        /// <remarks>
        /// INSERT: objeto criado, com o Id gerado. UPDATE: campos alterados com os valores gravados.
        /// DELETE e LOGIN: vazio. Dados sensíveis são gravados como "***".
        /// </remarks>
        [Column("novos_valores")]
        public string NovosValores { get; set; } = string.Empty;

        /// <summary>
        /// Chave estrangeira do Usuário que executou a ação.
        /// </summary>
        /// <remarks>
        /// Obtida do token JWT da requisição (ou do próprio usuário no LOGIN).
        /// Nulo quando a operação ocorreu sem usuário autenticado, como no cadastro público,
        /// na redefinição de senha ou no modo DEV sem token.
        /// </remarks>
        [Column("usuario_id")]
        public int? UsuarioId { get; set; }

        /// <summary>
        /// Objeto de navegação do Usuário autor da ação.
        /// </summary>
        [ForeignKey("UsuarioId")]
        public virtual Usuario? Usuario { get; set; }

        /// <summary>
        /// Construtor vazio para o Entity Framework.
        /// </summary>
        public Auditoria()
        {
        }

        /// <summary>
        /// Construtor completo para criação de um log de auditoria. O Id é gerado pelo banco.
        /// </summary>
        public Auditoria(string data, string hora, string operacao, string nomeEntidade,
            string valoresAtingidos, string novosValores, int? usuarioId)
        {
            Data = data;
            Hora = hora;
            Operacao = operacao;
            NomeEntidade = nomeEntidade;
            ValoresAtingidos = valoresAtingidos;
            NovosValores = novosValores;
            UsuarioId = usuarioId;
        }
    }
}
