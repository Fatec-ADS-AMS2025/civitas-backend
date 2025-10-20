using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Data.Repositories;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class InstituicaoService : GenericService<Instituicao, InstituicaoDTO>, IInstituicaoService
    {
        private readonly IInstituicaoRepository _instituicaoRepository;
        private readonly IMapper _mapper;

        public InstituicaoService(IInstituicaoRepository instituicaoRepository, IMapper mapper)
            : base(instituicaoRepository, mapper)
        {
            _instituicaoRepository = instituicaoRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InstituicaoDTO>> GetInstituicaoByName(string name)
        {
            var isntituicao = await _instituicaoRepository.GetInstituicaoByName(name);
            return _mapper.Map<IEnumerable<InstituicaoDTO>>(isntituicao);
        }
    }
}
