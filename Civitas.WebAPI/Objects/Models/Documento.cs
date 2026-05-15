using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Models
{
    /// <summary>
    /// Representa um arquivo digitalizado, comprovante ou anexo armazenado no sistema.
    /// Mapeia a tabela 'documento' do banco de dados.
    /// </summary>
    /// <remarks>
    /// Esta entidade é responsável por garantir a rastreabilidade e auditoria financeira, 
    /// armazenando a cópia digital (Blob) de Notas Fiscais, Boletos ou Contratos.
    /// </remarks>
    [Table("documento")]
    public class Documento : ISoftDeletable
    {
        /// <summary>
        /// Identificador único do documento (Chave Primária).
        /// </summary>
        [Column("iddocumento")]
        public int IdDocumento { get; set; }

        /// <summary>
        /// Conteúdo binário do arquivo (Blob).
        /// </summary>
        /// <remarks>
        /// Armazena o arquivo em formato de array de bytes.
        /// Idealmente utilizado para arquivos .PDF, .JPG ou .PNG de tamanho controlado.
        /// </summary>
        [Column("digitalizacao")]
        public byte[] Digitalizacao { get; set; }

        /// <summary>
        /// Número oficial impresso no documento físico.
        /// </summary>
        /// <example>Número da Nota Fiscal (12345) ou Código de Barras do Boleto.</example>
        /// <remarks>
        /// Essencial para conciliação bancária e identificação única junto ao fornecedor.
        /// </remarks>
        [Column("numerodocumento")]
        public int NumeroDocumento { get; set; }

        /// <summary>
        /// Chave estrangeira do Fornecedor emissor deste documento.
        /// </summary>
        [Column("idfornecedor")]
        public int IdFornecedor { get; set; }

        /// <summary>
        /// Objeto de navegação do Fornecedor vinculado.
        /// </summary>
        public Fornecedor Fornecedor { get; set; }

        /// <summary>
        /// Construtor para inicialização básica da entidade Documento.
        /// </summary>

        [Column("excluido")]
        public bool Excluido { get; set; }

        [Column("dataexclusao")]
        public DateTime? DataExclusao { get; set; }

        public Documento(int idDocumento, byte[] digitalizacao, int numeroDocumento)
        {
            IdDocumento = idDocumento;
            Digitalizacao = digitalizacao;
            NumeroDocumento = numeroDocumento;
        }
    }
}
