using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Data.Repositories;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço especializado na gestão de Instituições (Unidades Administrativas) do sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Centralizar regras de negócio para cadastro de escolas, postos de saúde e órgãos públicos.
    /// - Fornecer métodos de busca otimizados, além do CRUD padrão herdado.
    /// 
    /// Dependências:
    /// - <see cref="IInstituicaoRepository"/>: Acesso a dados com filtros específicos.
    /// - <see cref="IMapper"/>: Transformação de objetos.
    /// </remarks>
    public class InstituicaoService : GenericService<Instituicao, InstituicaoDTO>, IInstituicaoService
    {
        private readonly IInstituicaoRepository _instituicaoRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Instituições.
        /// </summary>
        /// <param name="instituicaoRepository">Repositório concreto de instituições.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public InstituicaoService(IInstituicaoRepository instituicaoRepository, IMapper mapper)
            : base(instituicaoRepository, mapper)
        {
            _instituicaoRepository = instituicaoRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Realiza uma busca textual por instituições contendo o termo especificado.
        /// </summary>
        /// <param name="name">Nome ou parte do nome da instituição para filtragem.</param>
        /// <returns>Uma coleção de DTOs de instituições que correspondem ao critério de busca.</returns>
        /// <remarks>
        /// Utilidade:
        /// - Usado em campos de "Autocomplete" ou barras de pesquisa no front-end.
        /// - A implementação no repositório geralmente ignora Case Sensitive (maiúsculas/minúsculas).
        /// </remarks>
        public async Task<IEnumerable<InstituicaoDTO>> GetInstituicaoByName(string name)
        {
            var isntituicao = await _instituicaoRepository.GetInstituicaoByName(name);
            return _mapper.Map<IEnumerable<InstituicaoDTO>>(isntituicao);
        }

        public async Task<InstituicaoGastosDTO?> GetGastosByInstituicaoIdAsync(int instituicaoId)
        {
            return await _instituicaoRepository.GetGastosByInstituicaoIdAsync(instituicaoId);
        }

        public async Task<InstituicaoOrcamentoDisponivelDTO?> GetOrcamentoDisponivelByInstituicaoIdAsync(int instituicaoId)
        {
            return await _instituicaoRepository.GetOrcamentoDisponivelByInstituicaoIdAsync(instituicaoId);
        }
    }
}