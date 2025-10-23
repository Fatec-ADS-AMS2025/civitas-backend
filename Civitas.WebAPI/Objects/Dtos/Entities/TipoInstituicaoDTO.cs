using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class TipoInstituicaoDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public Situacao Situacao { get; set; }
    }
}
