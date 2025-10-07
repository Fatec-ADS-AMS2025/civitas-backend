using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class UsuarioService : GenericService<Usuario, UsuarioDTO>, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        public UsuarioService(IUsuarioRepository usuarioRepository, IMapper mapper)
            : base(usuarioRepository, mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<UsuarioDTO>> GetUsuarioByCpf(string cpf)
        {
            var usuario = await _usuarioRepository.GetUsuarioByCpf(cpf);
            return _mapper.Map<IEnumerable<UsuarioDTO>>(usuario);
        }

        public async Task UpdateSituacao(int id, Situacao situacao)
        {
            if (!Enum.IsDefined(typeof(Situacao), situacao))
            {
                throw new ArgumentOutOfRangeException(nameof(situacao), $"Valor inválido: {situacao} apenas é aceito 1 (ativo) e 2 (negativo)");
            }

            var usuario = await _usuarioRepository.GetById(id);

            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuário com id {id} não foi encontrado.");
            }

            usuario.Situacao = situacao;
            await _usuarioRepository.Update(usuario);
        }
    }
}
