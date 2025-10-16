using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("documentos")]
    public class Documentos
    {
        [Column("idDocumentos")]
        public int IdDocumentos { get; set; }

        [Column("digitalizacao")]
        public Blob Digitalizacao { get; set; }

        [Column("numeroDocumento")]
        public int NumeroDocumento { get; set; }

        public Documentos(int idDocumentos, Blob digitalizacao, int numeroDocumento)
        {
            IdDocumentos = idDocumentos;
            Digitalizacao = digitalizacao;
            NumeroDocumento = numeroDocumento;
        }
    }
}