using Civitas.WebAPI.Objects.Enums;
using System.Reflection.Metadata;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class DocumentosDTO
    {
        public int IdDocumentos { get; set; }
        public Blob Digitalizacao { get; set; }
        public int NumeroDocumento { get; set; }
    }
}
