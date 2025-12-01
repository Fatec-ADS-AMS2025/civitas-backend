using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pelo gerenciamento das Unidades de Medida utilizadas no sistema.
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar o cadastro de grandezas físicas (ex: Quilowatts, Metros Cúbicos, Unidades).
    /// - Fornecer dados para as listas de seleção (dropdowns) na configuração de Tipos de Despesa.
    /// 
    /// Regras de Negócio:
    /// - As unidades gerenciadas aqui são utilizadas para calcular estatísticas de consumo e relatórios de eficiência.
    /// 
    /// Dependências:
    /// - <see cref="IUnidadeMedidaRepository"/>: Persistência de dados.
    /// - <see cref="IMapper"/>: Mapeamento de objetos.
    /// </remarks>
    public class UnidadeMedidaService : GenericService<UnidadeMedida, UnidadeMedidaDTO>, IUnidadeMedidaService
    {
        private readonly IUnidadeMedidaRepository _unidadeMedida;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Unidades de Medida.
        /// </summary>
        /// <param name="unidadeMedida">Repositório de unidades de medida.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public UnidadeMedidaService(IUnidadeMedidaRepository unidadeMedida, IMapper mapper)
            : base(unidadeMedida, mapper)
        {
            _unidadeMedida = unidadeMedida;
            _mapper = mapper;
        }

    }
}