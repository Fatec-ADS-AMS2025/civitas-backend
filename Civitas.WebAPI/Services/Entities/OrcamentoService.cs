using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pela gestão do planejamento orçamentário e tetos de gastos das instituições.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar a criação e manutenção dos orçamentos anuais.
    /// - Atuar como guardião da integridade financeira, validando se alterações no orçamento são permitidas.
    /// 
    /// Dependências:
    /// - <see cref="IOrcamentoRepository"/>: Acesso aos dados de orçamento.
    /// - <see cref="IMapper"/>: Transformação de objetos.
    /// </remarks>
    public class OrcamentoService : GenericService<Orcamento, OrcamentoDTO>, IOrcamentoService
    {
        private readonly IOrcamentoRepository _orcamentoRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de orçamentos.
        /// </summary>
        /// <param name="orcamentoRepository">Repositório de dados de orçamentos.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public OrcamentoService(IOrcamentoRepository orcamentoRepository, IMapper mapper)
            : base(orcamentoRepository, mapper)
        {
            _orcamentoRepository = orcamentoRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Verifica se existem despesas já lançadas vinculadas a um determinado orçamento.
        /// </summary>
        /// <param name="idOrcamento">O identificador do orçamento a ser verificado.</param>
        /// <returns>
        /// Retorna <c>true</c> se houver pelo menos uma despesa vinculada; caso contrário, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Regra de Negócio (Integridade Referencial):
        /// Este método deve ser chamado antes de operações de Exclusão (Delete).
        /// Se retornar verdadeiro, o sistema deve BLOQUEAR a exclusão do orçamento para manter o histórico financeiro consistente.
        /// </remarks>
        public async Task<bool> ExisteDespesaVinculada(int idOrcamento)
        {
            return await _orcamentoRepository.ExisteDespesaVinculada(idOrcamento);
        }
    }
}