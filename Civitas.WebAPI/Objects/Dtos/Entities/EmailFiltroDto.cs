namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Filtros opcionais da listagem geral de e-mails enviados.
    /// </summary>
    public class EmailFiltroDto
    {
        public string? EmailDestinatario { get; set; }

        public string? Status { get; set; }
    }
}
