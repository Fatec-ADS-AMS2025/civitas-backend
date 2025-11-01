using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class UnidadeMedidaService : GenericService<UnidadeMedida, UnidadeMedidaDTO>, IUnidadeMedidaService
    {
        private readonly IUnidadeMedidaRepository _unidadeMedida;
        private readonly IMapper _mapper;

        public UnidadeMedidaService(IUnidadeMedidaRepository unidadeMedida, IMapper mapper)
            : base(unidadeMedida, mapper)
        {
            _unidadeMedida = unidadeMedida;
            _mapper = mapper;
        }

    }
}
