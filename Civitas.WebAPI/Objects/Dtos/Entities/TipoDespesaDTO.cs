using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class TipoDespesaDTO
    {
        public int Id { get; set; }
        public string Descricao { get; set; }
        public SolicitaUc SolicitaUc { get; set; }
        public Situacao Situacao { get; set; }
        public int IdUnidadeMedida { get; set; }
    }
}
