using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pela gestão do ciclo de vida de arquivos digitais e comprovantes no sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar a persistência e recuperação de arquivos digitalizados (Blobs).
    /// - Vincular documentos fiscais (Notas, Boletos) aos fluxos de pagamento e fornecedores.
    /// 
    /// Regras de Negócio:
    /// - A manipulação de arquivos grandes (byte arrays) ocorre através deste serviço.
    /// - Atua como garantidor da auditoria financeira, provendo a evidência física dos pagamentos.
    /// 
    /// Dependências:
    /// - <see cref="IDocumentoRepository"/> para acesso ao banco de dados.
    /// - <see cref="IMapper"/> para conversão de DTOs (Upload/Download).
    /// </remarks>
    public class DocumentoService : GenericService<Documento, DocumentoDTO>, IDocumentoService
    {
        private readonly IDocumentoRepository _documentoRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de documentos com as dependências necessárias.
        /// </summary>
        /// <param name="documentoRepository">Repositório especializado na tabela 'documento'.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        /// <exception cref="ArgumentNullException">Lançada caso a injeção de dependência falhe.</exception>
        public DocumentoService(IDocumentoRepository documentoRepository, IMapper mapper)
            : base(documentoRepository, mapper)
        {
            _documentoRepository = documentoRepository;
            _mapper = mapper;
        }
    }
}