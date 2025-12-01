using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pela gestão das Categorias de Despesa (Configurações de Lançamento).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Gerenciar os tipos de contas permitidos no sistema (Água, Luz, Telefone).
    /// - Centralizar a configuração de regras de preenchimento (se exige UC, qual unidade de medida usa).
    /// 
    /// Dependências:
    /// - <see cref="ITipoDespesaRepository"/>: Acesso a dados.
    /// - <see cref="IMapper"/>: Mapeamento de objetos.
    /// </remarks>
    public class TipoDespesaService : GenericService<TipoDespesa, TipoDespesaDTO>, ITipoDespesaService
    {
        private readonly ITipoDespesaRepository _tipoDespesa;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa o serviço de Tipos de Despesa.
        /// </summary>
        /// <param name="tipoDespesa">Repositório concreto de tipos de despesa.</param>
        /// <param name="mapper">Mapeador de objetos.</param>
        public TipoDespesaService(ITipoDespesaRepository tipoDespesa, IMapper mapper)
            : base(tipoDespesa, mapper)
        {
            _tipoDespesa = tipoDespesa;
            _mapper = mapper;
        }

        /// <summary>
        /// Verifica a validade da Unidade de Medida vinculada a um tipo de despesa.
        /// </summary>
        /// <param name="idTipoDespesa">O identificador do Tipo de Despesa a ser verificado.</param>
        /// <returns>
        /// Retorna <c>true</c> se a unidade de medida vinculada estiver ATIVA.
        /// Retorna <c>false</c> se a unidade de medida estiver INATIVA ou inexistente.
        /// </returns>
        /// <remarks>
        /// Regra de Negócio:
        /// Antes de permitir o cadastro de uma nova despesa (ex: Energia), o sistema deve validar se a unidade usada (ex: kWh) ainda é válida.
        /// Isso evita inconsistências em relatórios de consumo caso uma unidade seja depreciada.
        /// </remarks>
        public async Task<bool> ExisteUnidadesDeMedidaAtivas(int idTipoDespesa)
        {
            return await _tipoDespesa.ExisteUnidadesDeMedidaAtivas(idTipoDespesa);
        }
    }
}