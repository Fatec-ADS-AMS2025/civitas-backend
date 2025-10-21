using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using System.Runtime.ConstrainedExecution;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("documento")]
    public class Documento
    {
        [Column("IdDocumento")]
        public int IdDocumento { get; set; }

        [Column("Digitalizacao")]
        public Byte[] Digitalizacao { get; set; }

        [Column("NumeroDocumento")]
        public int NumeroDocumento { get; set; }

        [Column("IdFornecedor")]
        public int IdFornecedor { get; set; }

        public Fornecedor Fornecedor { get; set; }


        public Documento(int idDocumento, byte[] digitalizacao, int numeroDocumento)
        {
            IdDocumento = idDocumento;
            Digitalizacao = digitalizacao;
            NumeroDocumento = numeroDocumento;
        }

    }
}