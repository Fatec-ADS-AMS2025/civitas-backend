using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class TipoCodigoService : GenericService<TipoCodigo, TipoCodigoDTO>, ITipoCodigoService
    {
        private readonly ITipoCodigoRepository _tipoCodigoRepository;
        private readonly IMapper _mapper;

        public TipoCodigoService(ITipoCodigoRepository tipoCodigoRepository, IMapper mapper)
            : base(tipoCodigoRepository, mapper)
        {
            _tipoCodigoRepository = tipoCodigoRepository;
            _mapper = mapper;
        }
    }
}
