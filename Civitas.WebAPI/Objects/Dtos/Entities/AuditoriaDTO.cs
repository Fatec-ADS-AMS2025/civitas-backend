using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência de dados para visualização e relatórios de Auditoria (Logs).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Output: Alimentar telas de histórico de alterações, permitindo rastrear quem fez o que e quando.
    /// - Input: Geralmente gerado automaticamente pelo backend, mas validado aqui caso haja inserção via API.
    /// </remarks>
    public class AuditoriaDTO
    {
        /// <summary>
        /// Identificador único do registro de log.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Data da ocorrência do evento.
        /// </summary>
        /// <remarks>
        /// Campo Obrigatório.
        /// </remarks>
        [Required(ErrorMessage = "A data é obrigatória")]
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Horário exato da ocorrência.
        /// </summary>
        /// <remarks>
        /// Campo Obrigatório.
        /// </remarks>
        [Required(ErrorMessage = "A hora é obrigatória")]
        public string Hora { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de ação executada.
        /// </summary>
        /// <remarks>
        /// Exemplos: INSERT, UPDATE, DELETE, LOGIN.
        /// Validação: Máximo de 100 caracteres.
        /// </remarks>
        [Required(ErrorMessage = "A operação é obrigatória")]
        [StringLength(100, ErrorMessage = "A operação deve ter no máximo 100 caracteres")]
        public string Operacao { get; set; } = string.Empty;

        /// <summary>
        /// Nome da tabela ou objeto afetado.
        /// </summary>
        /// <remarks>
        /// Validação: Máximo de 100 caracteres.
        /// </remarks>
        [Required(ErrorMessage = "O nome da entidade é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome da entidade deve ter no máximo 100 caracteres")]
        public string NomeEntidade { get; set; } = string.Empty;

        /// <summary>
        /// Snapshot dos dados ANTES da alteração (Valor Antigo).
        /// </summary>
        /// <remarks>
        /// Validação: Limite de 500 caracteres. Se o JSON do objeto for maior, deve ser truncado ou tratado.
        /// </remarks>
        [StringLength(500, ErrorMessage = "Os valores atingidos devem ter no máximo 500 caracteres")]
        public string ValoresAtingidos { get; set; } = string.Empty;

        /// <summary>
        /// Snapshot dos dados DEPOIS da alteração (Novo Valor).
        /// </summary>
        /// <remarks>
        /// Validação: Limite de 500 caracteres.
        /// </remarks>
        [StringLength(500, ErrorMessage = "Os novos valores devem ter no máximo 500 caracteres")]
        public string NovosValores { get; set; } = string.Empty;

        /// <summary>
        /// Estado do registro de auditoria.
        /// </summary>
        /// <remarks>
        /// Valores: <see cref="Situacao"/>.
        /// </remarks>
        [Required(ErrorMessage = "A situação é obrigatória")]
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Identificador do Usuário que realizou a operação.
        /// </summary>
        /// <remarks>
        /// Campo Obrigatório. Garante a rastreabilidade do autor.
        /// </remarks>
        [Required(ErrorMessage = "O ID do usuário é obrigatório")]
        public int UsuarioId { get; set; }

        /// <summary>
        /// Objeto aninhado com os dados detalhados do Usuário.
        /// </summary>
        /// <remarks>
        /// Output: Preenchido em consultas para exibir o nome/login do responsável sem necessidade de nova requisição.
        /// </remarks>
        public UsuarioDTO? Usuario { get; set; }
    }
}