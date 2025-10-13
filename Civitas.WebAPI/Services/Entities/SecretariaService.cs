using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class SecretariaService : GenericService<Secretaria, SecretariaDTO>, ISecretariaService
    {
        private readonly ISecretariaRepository _secretariaRepository;
        private readonly IMapper _mapper;

        public SecretariaService(ISecretariaRepository secretariaRepository, IMapper mapper)
            : base(secretariaRepository, mapper)
        {
            _secretariaRepository = secretariaRepository;
            _mapper = mapper;
        }
    }
}
