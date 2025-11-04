using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class UnidadeMedidaDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public string Abreviatura { get; set; }
        public Situacao Situacao { get; set; }
    }
}
