using Civitas.WebAPI.Objects.Enums;
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
    /// </remarks>
    [Table("auditoria")]
    public class Auditoria
    {
        /// <summary>
        /// Identificador único do registro de log (Chave Primária).
        /// </summary>
        [Column("id")]
        public int Id { get; set; }

        /// <summary>
        /// Data em que a operação ocorreu.
        /// </summary>
        /// <remarks>
        /// Armazenado como string. Recomendado formato ISO (AAAA-MM-DD) para facilitar ordenação.
        /// </remarks>
        [Column("data")]
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Horário exato em que a operação ocorreu.
        /// </summary>
        /// <example>14:30:59</example>
        [Column("hora")]
        public string Hora { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de operação realizada no banco de dados.
        /// </summary>
        /// <example>INSERT, UPDATE, DELETE, LOGIN.</example>
        [Column("operacao")]
        public string Operacao { get; set; } = string.Empty;

        /// <summary>
        /// Nome da tabela ou entidade que sofreu a alteração.
        /// </summary>
        /// <example>Usuario, Despesa, Fornecedor.</example>
        [Column("nome_entidade")]
        public string NomeEntidade { get; set; } = string.Empty;

        /// <summary>
        /// Snapshot dos dados ANTES da alteração (Valores Antigos).
        /// </summary>
        /// <remarks>
        /// Geralmente armazenado em formato JSON stringificado para permitir a reconstrução do estado anterior.
        /// </remarks>
        [Column("valores_atingidos")]
        public string ValoresAtingidos { get; set; } = string.Empty;

        /// <summary>
        /// Snapshot dos dados DEPOIS da alteração (Novos Valores).
        /// </summary>
        /// <remarks>
        /// Em operações de INSERT, contém o objeto criado. Em UPDATE, o objeto atualizado. Em DELETE, pode estar vazio ou nulo.
        /// </remarks>
        [Column("novos_valores")]
        public string NovosValores { get; set; } = string.Empty;

        /// <summary>
        /// Estado do registro de auditoria.
        /// </summary>
        /// <remarks>
        /// Controlado pelo Enum <see cref="Situacao"/>. 
        /// Geralmente registros de auditoria são sempre criados como ATIVOS e nunca devem ser inativados para garantir a integridade histórica.
        /// </remarks>
        [Column("situacao")]
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Chave estrangeira do Usuário que executou a ação.
        /// </summary>
        /// <remarks>
        /// Identifica o autor da mudança (Responsabilidade).
        /// </remarks>
        [Column("usuario_id")]
        public int UsuarioId { get; set; }

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
        /// Construtor completo para criação de um log de auditoria.
        /// </summary>
        public Auditoria(int id, string data, string hora, string operacao, string nomeEntidade,
            string valoresAtingidos, string novosValores, Situacao situacao, int usuarioId)
        {
            Id = id;
            Data = data;
            Hora = hora;
            Operacao = operacao;
            NomeEntidade = nomeEntidade;
            ValoresAtingidos = valoresAtingidos;
            NovosValores = novosValores;
            Situacao = situacao;
            UsuarioId = usuarioId;
        }
    }
}