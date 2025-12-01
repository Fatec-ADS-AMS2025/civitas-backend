using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pelo gerenciamento da execução financeira (Fluxo de Caixa/Pagamentos).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar o ciclo de vida de uma parcela ou competência específica.
    /// - Controlar a mudança de status (A Pagar -> Paga -> Atrasado).
    /// - Registrar os valores reais pagos e o consumo aferido (ex: leitura do relógio de luz).
    /// 
    /// Regras de Negócio:
    /// - É através deste serviço que se confirma a quitação de uma obrigação gerada na Despesa.
    /// 
    /// Dependências:
    /// - <see cref="IFluxoRepository"/>: Camada de acesso a dados.
    /// - <see cref="IMapper"/>: Conversão entre Entidade e DTO.
    /// </remarks>
    public class FluxoService : GenericService<Fluxo, FluxoDTO>, IFluxoService
    {
        private readonly IFluxoRepository _fluxoRepository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Fluxo com as dependências necessárias.
        /// </summary>
        /// <param name="fluxoRepository">Repositório concreto para persistência de fluxos.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        /// <exception cref="ArgumentNullException">Disparada caso as dependências não sejam resolvidas corretamente.</exception>
        public FluxoService(IFluxoRepository fluxoRepository, IMapper mapper)
            : base(fluxoRepository, mapper)
        {
            _fluxoRepository = fluxoRepository;
            _mapper = mapper;
        }
    }
}