using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência de arquivos PDFs.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Output: Abrir documento PDF referente a uma despesa específica.
    /// </remarks>
    public class FileResultDto
    {
        public Stream Stream { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; } = "application/pdf";
        public bool Inline { get; set; } = true;
    }
}
