using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pela gestão das Categorias de Instituição.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar a taxonomia/classificação das instituições (ex: Escola, Hospital, ONG).
    /// - Garantir a integridade dos dados impedindo a exclusão de categorias em uso.
    /// 
    /// Dependências:
    /// - <see cref="ITipoInstituicaoRepository"/>: Persistência de dados.
    /// - <see cref="IMapper"/>: Transformação de objetos.
    /// </remarks>
    public class TipoInstituicaoService : GenericService<TipoInstituicao, TipoInstituicaoDTO>, ITipoInstituicaoService
    {
        private readonly ITipoInstituicaoRepository _tipoInstituicao;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Tipos de Instituição.
        /// </summary>
        /// <param name="tipoInstituicao">Repositório concreto de tipos de instituição.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public TipoInstituicaoService(ITipoInstituicaoRepository tipoInstituicao, IMapper mapper)
            : base(tipoInstituicao, mapper)
        {
            _tipoInstituicao = tipoInstituicao;
            _mapper = mapper;
        }

        /// <summary>
        /// Verifica se existem instituições ativas vinculadas a uma determinada categoria.
        /// </summary>
        /// <param name="idTipoInstituicao">O identificador da categoria (Tipo) a ser verificada.</param>
        /// <returns>
        /// Retorna <c>true</c> se houver instituições vinculadas. 
        /// Retorna <c>false</c> se a categoria estiver livre para ser removida ou desativada.
        /// </returns>
        /// <remarks>
        /// Regra de Negócio (Safe Delete):
        /// Este método atua como uma trava de segurança. O sistema não deve permitir a exclusão de um Tipo de Instituição
        /// enquanto houver registros dependentes dele, para evitar "órfãos" no banco de dados.
        /// </remarks>
        public async Task<bool> ExisteInstituicoesAtivas(int idTipoInstituicao)
        {
            return await _tipoInstituicao.ExisteInstituicoesAtivasAsync(idTipoInstituicao);
        }
    }
}