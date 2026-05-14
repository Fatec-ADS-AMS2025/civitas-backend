namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class TipoCodigoDTO
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }
        public bool Excluido { get; set; }

        public DateTime? DataExclusao { get; set; }

    }
}

