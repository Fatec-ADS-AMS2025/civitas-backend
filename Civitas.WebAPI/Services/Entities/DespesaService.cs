using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class DespesaService : GenericService<Despesa, DespesaDTO>, IDespesaService
    {
        private readonly IDespesaRepository _despesaRepository;
        private readonly IMapper _mapper;

        public DespesaService(IDespesaRepository despesaRepository, IMapper mapper)
            : base(despesaRepository, mapper)
        {
            _despesaRepository = despesaRepository;
            _mapper = mapper;
        }
    }
}
