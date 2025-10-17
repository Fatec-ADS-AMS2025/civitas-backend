using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class AuditoriaDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "A data é obrigatória")]
        public string Data { get; set; } = string.Empty;

        [Required(ErrorMessage = "A hora é obrigatória")]
        public string Hora { get; set; } = string.Empty;

        [Required(ErrorMessage = "A operação é obrigatória")]
        [StringLength(100, ErrorMessage = "A operação deve ter no máximo 100 caracteres")]
        public string Operacao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O nome da entidade é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome da entidade deve ter no máximo 100 caracteres")]
        public string NomeEntidade { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Os valores atingidos devem ter no máximo 500 caracteres")]
        public string ValoresAtingidos { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Os novos valores devem ter no máximo 500 caracteres")]
        public string NovosValores { get; set; } = string.Empty;

        [Required(ErrorMessage = "A situação é obrigatória")]
        public Situacao Situacao { get; set; }

        [Required(ErrorMessage = "O ID do usuário é obrigatório")]
        public int UsuarioId { get; set; }

        // Propriedade de navegação para exibir dados do usuário
        public UsuarioDTO? Usuario { get; set; }
    }
}
