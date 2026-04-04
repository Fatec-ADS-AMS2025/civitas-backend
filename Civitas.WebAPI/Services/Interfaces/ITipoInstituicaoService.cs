using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gestão de Tipos de Instituição (Categorias).
    /// </summary>
    /// <remarks>
    /// Extende <see cref="IGenericService{TipoInstituicao, TipoInstituicaoDTO}"/> para incluir validações de dependência.
    /// Define as regras para a taxonomia das unidades administrativas (ex: Escola, Hospital, Prefeitura).
    /// </remarks>
    public interface ITipoInstituicaoService : IGenericService<TipoInstituicao, TipoInstituicaoDTO>
    {
        /// <summary>
        /// Verifica a existência de instituições ativas vinculadas a uma categoria específica.
        /// </summary>
        /// <param name="idTipoInstituicao">O identificador da categoria a ser verificada.</param>
        /// <returns>
        /// Uma tarefa contendo <c>true</c> se existirem instituições utilizando esta categoria; caso contrário, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Contrato de Integridade:
        /// Método essencial para implementar o "Safe Delete". O sistema não deve permitir a remoção de um tipo de instituição
        /// que ainda possua vínculos, evitando a criação de registros órfãos.
        /// </remarks>
        Task<bool> ExisteInstituicoesAtivas(int idTipoInstituicao);
    }
}