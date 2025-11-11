using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class FluxoService : GenericService<Fluxo, FluxoDTO>, IFluxoService
    {
        private readonly IFluxoRepository _fluxoRepository;
        private readonly IMapper _mapper;

        public FluxoService(IFluxoRepository fluxoRepository, IMapper mapper)
            : base(fluxoRepository, mapper)
        {
            _fluxoRepository = fluxoRepository;
            _mapper = mapper;
        }
    }
}
