using Civitas.WebAPI.Objects.Enums;
using System.Reflection.Metadata;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência para manipulação de arquivos digitais (Upload e Download).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Input: Receber o arquivo digitalizado (Blob) para anexar a um pagamento.
    /// - Output: Retornar o binário do arquivo para visualização ou download.
    /// </remarks>
    public class DocumentoDTO
    {
        /// <summary>
        /// Identificador único do documento.
        /// </summary>
        /// <remarks>
        /// Input: Ignorado no momento do upload (criação).
        /// </remarks>
        public int IdDocumento { get; set; }

        /// <summary>
        /// Conteúdo binário do arquivo (Array de Bytes).
        /// </summary>
        /// <remarks>
        /// Obrigatório. Representa o arquivo físico digitalizado (PDF, JPG, PNG).
        /// Validação de Performance: O backend deve validar o tamanho máximo permitido (ex: 5MB) para evitar sobrecarga de memória.
        /// </remarks>
        public byte[] Digitalizacao { get; set; }

        /// <summary>
        /// Número oficial impresso no documento.
        /// </summary>
        /// <example>102030 (Número da Nota Fiscal).</example>
        /// <remarks>
        /// Usado para conferência e auditoria.
        /// </remarks>
        public int NumeroDocumento { get; set; }

        /// <summary>
        /// Identificador do Fornecedor emissor do documento.
        /// </summary>
        /// <remarks>
        /// Obrigatório.
        /// </remarks>
        public int IdFornecedor { get; set; }

        /// <summary>
        /// Identificador do Fluxo (Pagamento/Medição) vinculado.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Define qual parcela ou competência este documento está comprovando.
        /// </remarks>
        public int IdFluxo { get; set; }
    }
}