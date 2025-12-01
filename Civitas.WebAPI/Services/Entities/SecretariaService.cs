using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pela administração das Secretarias e órgãos gestores superiores.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar o cadastro das entidades que agrupam as instituições (ex: Secretaria de Educação).
    /// - Centralizar a gestão de dados fiscais (CNPJ) e contatos dos órgãos públicos.
    /// 
    /// Regras de Negócio:
    /// - Uma Secretaria atua como "pai" na hierarquia, sendo mandatória para a criação de Instituições.
    /// 
    /// Dependências:
    /// - <see cref="ISecretariaRepository"/>: Camada de persistência.
    /// - <see cref="IMapper"/>: Conversão de dados.
    /// </remarks>
    public class SecretariaService : GenericService<Secretaria, SecretariaDTO>, ISecretariaService
    {
        private readonly ISecretariaRepository _secretariaRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Secretarias.
        /// </summary>
        /// <param name="secretariaRepository">Repositório injetado para acesso a dados.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        /// <exception cref="ArgumentNullException">Lançada se as dependências não forem resolvidas.</exception>
        public SecretariaService(ISecretariaRepository secretariaRepository, IMapper mapper)
            : base(secretariaRepository, mapper)
        {
            _secretariaRepository = secretariaRepository;
            _mapper = mapper;
        }
    }
}