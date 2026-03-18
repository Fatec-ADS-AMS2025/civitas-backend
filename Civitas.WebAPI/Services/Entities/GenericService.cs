using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço genérico base que implementa as operações padrão de CRUD (Create, Read, Update, Delete).
    /// </summary>
    /// <typeparam name="T">Tipo da Entidade de domínio (Model) mapeada no banco.</typeparam>
    /// <typeparam name="TDto">Tipo do Objeto de Transferência (DTO) exposto para a API.</typeparam>
    /// <remarks>
    /// Finalidade:
    /// - Centralizar a lógica repetitiva de acesso a dados e mapeamento de objetos.
    /// - Garantir que todas as entidades do sistema tenham operações básicas padronizadas.
    /// 
    /// Dependências:
    /// - <see cref="IGenericRepository{T}"/>: Para persistência no banco de dados.
    /// - <see cref="IMapper"/>: Para conversão bidirecional entre T e TDto.
    /// </remarks>
    public class GenericService<T, TDto> : IGenericService<T, TDto> where T : class where TDto : class
    {
        private readonly IGenericRepository<T> _repository;
        private readonly IMapper _mapper;

        /// <summary>
        /// Inicializa a instância do serviço genérico.
        /// </summary>
        /// <param name="repository">Instância do repositório genérico injetado.</param>
        /// <param name="mapper">Instância do AutoMapper injetado.</param>
        public GenericService(IGenericRepository<T> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Obtém todos os registros da entidade.
        /// </summary>
        /// <returns>Uma coleção assíncrona contendo todos os registros convertidos para DTO.</returns>
        public virtual async Task<IEnumerable<TDto>> GetAll()
        {
            var entities = await _repository.Get();
            return _mapper.Map<IEnumerable<TDto>>(entities);
        }

        /// <summary>
        /// Obtém uma página de registros com metadados de paginação.
        /// </summary>
        /// <param name="paginationQuery">Parâmetros da consulta paginada.</param>
        /// <returns>Resultado paginado convertido para DTO.</returns>
        public virtual async Task<PaginatedResult<TDto>> GetPage(PaginationQuery paginationQuery)
        {
            var entities = await _repository.GetPage(paginationQuery);
            var items = _mapper.Map<List<TDto>>(entities.Items);

            return new PaginatedResult<TDto>
            {
                Items = items,
                TotalRecords = entities.TotalRecords,
                TotalPages = entities.TotalPages,
                CurrentPage = entities.CurrentPage,
                PageSize = entities.PageSize
            };
        }

        /// <summary>
        /// Obtém um registro específico pelo seu identificador único.
        /// </summary>
        /// <param name="id">O ID do registro a ser buscado.</param>
        /// <returns>O objeto DTO correspondente ou null se não encontrado.</returns>
        public async Task<TDto> GetById(int id)
        {
            var entity = await _repository.GetById(id);
            return _mapper.Map<TDto>(entity);
        }

        /// <summary>
        /// Cria um novo registro no banco de dados.
        /// </summary>
        /// <param name="entityDTO">O objeto DTO contendo os dados para criação.</param>
        /// <remarks>
        /// O método converte o DTO para a Entidade de domínio antes de salvar.
        /// </remarks>
        public virtual async Task Create(TDto entityDTO)
        {
            var entity = _mapper.Map<T>(entityDTO);
            await _repository.Add(entity);
        }

        /// <summary>
        /// Atualiza um registro existente.
        /// </summary>
        /// <param name="entityDTO">O objeto DTO com os novos dados.</param>
        /// <param name="id">O ID do registro a ser atualizado.</param>
        /// <exception cref="KeyNotFoundException">Lançada se não existir um registro com o ID informado no banco de dados.</exception>
        public virtual async Task Update(TDto entityDTO, int id)
        {
            var existingEntity = await _repository.GetById(id);

            if (existingEntity == null)
            {
                throw new KeyNotFoundException($"Entity with id {id} not found.");
            }

            var entity = _mapper.Map<T>(entityDTO);
            await _repository.Update(entity);
        }

        /// <summary>
        /// Remove um registro do banco de dados.
        /// </summary>
        /// <param name="id">O ID do registro a ser excluído.</param>
        /// <exception cref="KeyNotFoundException">Lançada se a entidade não for encontrada antes da exclusão.</exception>
        public async Task Remove(int id)
        {
            var entity = await _repository.GetById(id);
            if (entity == null)
            {
                throw new KeyNotFoundException($"Entidade com id: {id} não encontrado");
            }

            await _repository.Remove(entity);
        }
    }
}
