using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pela gestão de usuários, credenciais e perfis de acesso.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Centralizar a lógica de cadastro, atualização e consulta de usuários do sistema.
    /// - Gerenciar os perfis de acesso (Visitante, Administrador, Funcionário).
    /// 
    /// Regras de Negócio:
    /// - O CPF deve ser único no sistema.
    /// - Este serviço é a base para as rotinas de Autenticação e Autorização.
    /// 
    /// Dependências:
    /// - <see cref="IUsuarioRepository"/>: Camada de acesso a dados.
    /// - <see cref="IMapper"/>: Transformação de objetos (DTO/Entity).
    /// </remarks>
    public class UsuarioService : GenericService<Usuario, UsuarioDTO>, IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Usuários.
        /// </summary>
        /// <param name="usuarioRepository">Repositório de usuários injetado.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public UsuarioService(IUsuarioRepository usuarioRepository, IMapper mapper)
            : base(usuarioRepository, mapper)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Realiza a busca de usuários através do número de CPF.
        /// </summary>
        /// <param name="cpf">O número do Cadastro de Pessoa Física a ser pesquisado.</param>
        /// <returns>Uma coleção de usuários encontrados (geralmente contendo 0 ou 1 registro).</returns>
        /// <remarks>
        /// Utilidade:
        /// - Verificação de duplicidade no momento do cadastro (impedir dois usuários com mesmo CPF).
        /// - Recuperação de conta ou login alternativo.
        /// </remarks>
        public async Task<IEnumerable<UsuarioDTO>> GetUsuarioByCpf(string cpf)
        {
            var usuario = await _usuarioRepository.GetUsuarioByCpf(cpf);
            return _mapper.Map<IEnumerable<UsuarioDTO>>(usuario);
        }
    }
}