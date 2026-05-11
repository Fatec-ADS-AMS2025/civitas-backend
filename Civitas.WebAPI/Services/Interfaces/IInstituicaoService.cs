using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gestão de Instituições (Unidades Administrativas).
    /// </summary>
    /// <remarks>
    /// Extende <see cref="IGenericService{Instituicao, InstituicaoDTO}"/> para incluir operações específicas de domínio,
    /// como buscas personalizadas necessárias para o frontend.
    /// </remarks>
    public interface IInstituicaoService : IGenericService<Instituicao, InstituicaoDTO>
    {
        /// <summary>
        /// Realiza uma busca textual por instituições baseada no nome.
        /// </summary>
        /// <param name="nome">O nome ou fragmento do nome para a pesquisa.</param>
        /// <returns>Uma coleção de DTOs de instituições que correspondem ao critério.</returns>
        /// <remarks>
        /// Contrato utilizado para implementar barras de pesquisa ou autocomplete.
        /// </remarks>
        Task<IEnumerable<InstituicaoDTO>> GetInstituicaoByName(string nome);
        Task<InstituicaoGastosDTO?> GetGastosByInstituicaoIdAsync(int instituicaoId, int tipoDespesaId);
        Task<InstituicaoOrcamentoDisponivelDTO?> GetOrcamentoDisponivelByInstituicaoIdAsync(int instituicaoId);
    }
}