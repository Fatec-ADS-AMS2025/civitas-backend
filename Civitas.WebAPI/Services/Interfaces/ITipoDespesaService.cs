using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gestão de Tipos de Despesa (Configurações de Lançamento).
    /// </summary>
    /// <remarks>
    /// Extende <see cref="IGenericService{TipoDespesa, TipoDespesaDTO}"/>.
    /// Atua como contrato para a lógica que define COMO uma despesa deve ser lançada (unidade de medida, obrigatoriedade de UC, etc.).
    /// </remarks>
    public interface ITipoDespesaService : IGenericService<TipoDespesa, TipoDespesaDTO>
    {
        /// <summary>
        /// Verifica se o Tipo de Despesa está vinculado a uma Unidade de Medida válida e ativa.
        /// </summary>
        /// <param name="idTipoDespesa">O identificador do tipo de despesa a ser validado.</param>
        /// <returns>
        /// Uma tarefa contendo <c>true</c> se a unidade de medida vinculada estiver ativa; caso contrário, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Contrato de Validação:
        /// Utilizado para garantir que novas despesas não sejam criadas utilizando unidades de medida depreciadas (Inativas).
        /// </remarks>
        Task<bool> ExisteUnidadesDeMedidaAtivas(int idTipoDespesa);
        Task<bool> HasDespesasVinculadas(int idTipoDespesa);
    }
}