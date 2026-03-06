using Civitas.WebAPI.Objects.Contracts;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface genérica que define o contrato padrão de serviços para operações CRUD (Create, Read, Update, Delete).
    /// </summary>
    /// <typeparam name="T">Tipo da Entidade de Domínio (Model) mapeada no banco de dados.</typeparam>
    /// <typeparam name="TDto">Tipo do Objeto de Transferência de Dados (DTO) exposto pela API.</typeparam>
    /// <remarks>
    /// Esta interface garante que todos os serviços do sistema tenham uma assinatura consistente para as operações básicas,
    /// facilitando a injeção de dependência e a manutenção do código.
    /// </remarks>
    public interface IGenericService<T, TDto> where T : class where TDto : class
    {
        /// <summary>
        /// Recupera todos os registros da entidade disponíveis no banco de dados.
        /// </summary>
        /// <returns>Uma tarefa assíncrona contendo a coleção de objetos convertidos para DTO.</returns>
        Task<IEnumerable<TDto>> GetAll();

        /// <summary>
        /// Recupera uma página específica com metadados de paginação e ordenação.
        /// </summary>
        /// <param name="paginationQuery">Parâmetros da consulta paginada.</param>
        /// <returns>Uma tarefa assíncrona contendo o resultado paginado.</returns>
        Task<PaginatedResult<TDto>> GetPage(PaginationQuery paginationQuery);

        /// <summary>
        /// Busca um registro específico pelo seu identificador único.
        /// </summary>
        /// <param name="id">O ID do registro a ser recuperado.</param>
        /// <returns>A tarefa contendo o DTO encontrado ou null caso não exista.</returns>
        Task<TDto> GetById(int id);

        /// <summary>
        /// Cria um novo registro no banco de dados com base no DTO fornecido.
        /// </summary>
        /// <param name="entityDTO">O objeto contendo os dados para criação.</param>
        /// <returns>Uma tarefa assíncrona.</returns>
        Task Create(TDto entityDTO);

        /// <summary>
        /// Atualiza os dados de um registro existente.
        /// </summary>
        /// <param name="entityDTO">O objeto DTO com os novos dados atualizados.</param>
        /// <param name="id">O identificador do registro que será modificado.</param>
        /// <returns>Uma tarefa assíncrona.</returns>
        /// <remarks>
        /// As implementações devem verificar se o registro existe antes de tentar atualizar.
        /// </remarks>
        Task Update(TDto entityDTO, int id);

        /// <summary>
        /// Remove permanentemente um registro do banco de dados.
        /// </summary>
        /// <param name="id">O identificador do registro a ser excluído.</param>
        /// <returns>Uma tarefa assíncrona.</returns>
        Task Remove(int id);
    }
}
