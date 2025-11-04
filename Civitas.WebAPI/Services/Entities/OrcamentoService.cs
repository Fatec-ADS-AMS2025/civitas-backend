using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class OrcamentoService : GenericService<Orcamento, OrcamentoDTO>, IOrcamentoService
    {
        private readonly IOrcamentoRepository _orcamentoRepository;
        private readonly IMapper _mapper;

        public OrcamentoService(IOrcamentoRepository orcamentoRepository, IMapper mapper)
            : base(orcamentoRepository, mapper)
        {
            _orcamentoRepository = orcamentoRepository;
            _mapper = mapper;
        }
    }
}
