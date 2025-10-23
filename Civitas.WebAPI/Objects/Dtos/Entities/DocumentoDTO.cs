using Civitas.WebAPI.Objects.Enums;
using System.Reflection.Metadata;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    public class DocumentoDTO
    {
        public int IdDocumento { get; set; }
        public byte[] Digitalizacao { get; set; }
        public int NumeroDocumento { get; set; }
        public int IdFornecedor { get; set; }
    }
}
