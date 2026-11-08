using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferÃªncia de dados para visualizaÃ§Ã£o e relatÃ³rios de Auditoria (Logs).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Output: Alimentar telas de histÃ³rico de alteraÃ§Ãµes, permitindo rastrear quem fez o que e quando.
    /// - Input: Geralmente gerado automaticamente pelo backend, mas validado aqui caso haja inserÃ§Ã£o via API.
    /// </remarks>
    public class AuditoriaDTO
    {
        /// <summary>
        /// Identificador Ãºnico do registro de log.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Data da ocorrÃªncia do evento.
        /// </summary>
        /// <remarks>
        /// Campo ObrigatÃ³rio.
        /// </remarks>
        [Required(ErrorMessage = "A data Ã© obrigatÃ³ria")]
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// HorÃ¡rio exato da ocorrÃªncia.
        /// </summary>
        /// <remarks>
        /// Campo ObrigatÃ³rio.
        /// </remarks>
        [Required(ErrorMessage = "A hora Ã© obrigatÃ³ria")]
        public string Hora { get; set; } = string.Empty;

        /// <summary>
        /// Tipo de aÃ§Ã£o executada.
        /// </summary>
        /// <remarks>
        /// Exemplos: INSERT, UPDATE, DELETE, LOGIN.
        /// ValidaÃ§Ã£o: MÃ¡ximo de 100 caracteres.
        /// </remarks>
        [Required(ErrorMessage = "A operaÃ§Ã£o Ã© obrigatÃ³ria")]
        [StringLength(100, ErrorMessage = "A operaÃ§Ã£o deve ter no mÃ¡ximo 100 caracteres")]
        public string Operacao { get; set; } = string.Empty;

        /// <summary>
        /// Nome da tabela ou objeto afetado.
        /// </summary>
        /// <remarks>
        /// ValidaÃ§Ã£o: MÃ¡ximo de 100 caracteres.
        /// </remarks>
        [Required(ErrorMessage = "O nome da entidade Ã© obrigatÃ³rio")]
        [StringLength(100, ErrorMessage = "O nome da entidade deve ter no mÃ¡ximo 100 caracteres")]
        public string NomeEntidade { get; set; } = string.Empty;

        /// <summary>
        /// Snapshot dos dados ANTES da alteraÃ§Ã£o (Valor Antigo).
        /// </summary>
        /// <remarks>
        /// ValidaÃ§Ã£o: Limite de 500 caracteres. Se o JSON do objeto for maior, deve ser truncado ou tratado.
        /// </remarks>
        [StringLength(500, ErrorMessage = "Os valores atingidos devem ter no mÃ¡ximo 500 caracteres")]
        public string ValoresAtingidos { get; set; } = string.Empty;

        /// <summary>
        /// Snapshot dos dados DEPOIS da alteraÃ§Ã£o (Novo Valor).
        /// </summary>
        /// <remarks>
        /// ValidaÃ§Ã£o: Limite de 500 caracteres.
        /// </remarks>
        [StringLength(500, ErrorMessage = "Os novos valores devem ter no mÃ¡ximo 500 caracteres")]
        public string NovosValores { get; set; } = string.Empty;

        /// <summary>
        /// Identificador do UsuÃ¡rio que realizou a operaÃ§Ã£o.
        /// </summary>
        /// <remarks>
        /// Campo ObrigatÃ³rio. Garante a rastreabilidade do autor.
        /// </remarks>
        [Required(ErrorMessage = "O ID do usuÃ¡rio Ã© obrigatÃ³rio")]
        public int UsuarioId { get; set; }

    }
}
