using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class AuditoriaService : GenericService<Auditoria, AuditoriaDTO>, IAuditoriaService
    {
        private readonly IAuditoriaRepository _auditoriaRepository;
        private readonly IMapper _mapper;

        public AuditoriaService(IAuditoriaRepository auditoriaRepository, IMapper mapper)
            : base(auditoriaRepository, mapper)
        {
            _auditoriaRepository = auditoriaRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AuditoriaDTO>> GetByUsuarioId(int usuarioId)
        {
            var auditorias = await _auditoriaRepository.GetByUsuarioId(usuarioId);
            return _mapper.Map<IEnumerable<AuditoriaDTO>>(auditorias);
        }

        public async Task<IEnumerable<AuditoriaDTO>> GetByEntidade(string nomeEntidade)
        {
            var auditorias = await _auditoriaRepository.GetByEntidade(nomeEntidade);
            return _mapper.Map<IEnumerable<AuditoriaDTO>>(auditorias);
        }

        public async Task<IEnumerable<AuditoriaDTO>> GetByOperacao(string operacao)
        {
            var auditorias = await _auditoriaRepository.GetByOperacao(operacao);
            return _mapper.Map<IEnumerable<AuditoriaDTO>>(auditorias);
        }
    }
}
