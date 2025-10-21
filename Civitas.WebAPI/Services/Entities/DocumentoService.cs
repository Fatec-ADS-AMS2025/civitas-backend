using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class DocumentoService : GenericService<Documento, DocumentoDTO>, IDocumentoService
    {
        private readonly IDocumentoRepository _documentoRepository;
        private readonly IMapper _mapper;

        public DocumentoService(IDocumentoRepository documentoRepository, IMapper mapper)
            : base(documentoRepository, mapper)
        {
            _documentoRepository = documentoRepository;
            _mapper = mapper;
        }
    }
}
