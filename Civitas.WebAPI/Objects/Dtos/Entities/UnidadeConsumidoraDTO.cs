namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// DTO de transferencia da entidade UnidadeConsumidora.
    /// </summary>
    public class UnidadeConsumidoraDTO
    {
        public int Id { get; set; }

        public string Identificador { get; set; } = string.Empty;

        public int IdInstituicao { get; set; }

        public int IdTipoDespesa { get; set; }

        public int IdSecretaria { get; set; }

        public int IdOrcamento { get; set; }

        public int IdFornecedor { get; set; }

        public bool Excluido { get; set; }

        public DateTime? DataExclusao { get; set; }
    }
}
