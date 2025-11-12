using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    [Table("documento")]
    public class Documento
    {
        [Column("iddocumento")]
        public int IdDocumento { get; set; }

        [Column("digitalizacao")]
        public byte[] Digitalizacao { get; set; }

        [Column("numerodocumento")]
        public int NumeroDocumento { get; set; }

        [Column("idfornecedor")]
        public int IdFornecedor { get; set; }
        public Fornecedor Fornecedor { get; set; }

        [Column("idfluxo")]
        public int IdFluxo { get; set; }
        public Fluxo Fluxo { get; set; }

        public Documento(int idDocumento, byte[] digitalizacao, int numeroDocumento)
        {
            IdDocumento = idDocumento;
            Digitalizacao = digitalizacao;
            NumeroDocumento = numeroDocumento;
        }
    }
}