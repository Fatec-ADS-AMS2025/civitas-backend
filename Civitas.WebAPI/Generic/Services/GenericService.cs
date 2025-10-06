using AutoMapper;
using Civitas.WebAPI.Generic.Models;
using Civitas.WebAPI.Generic.Repositories;

namespace Civitas.WebAPI.Generic.Services;

/// <summary>
/// Implementação genérica de serviço com operações CRUD básicas
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
/// <typeparam name="TDto">Tipo do DTO de resposta</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
public abstract class GenericService<TEntity, TDto, TCreateDto, TUpdateDto> : IGenericService<TDto, TCreateDto, TUpdateDto>
    where TEntity : BaseEntity
{
    protected readonly IGenericRepository<TEntity> _repository;
    protected readonly IMapper _mapper;

    protected GenericService(IGenericRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<IEnumerable<TDto>> GetAllAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<IEnumerable<TDto>>(entities);
    }

    public virtual async Task<IEnumerable<TDto>> GetActiveAsync()
    {
        var entities = await _repository.GetWhereAsync(e => e.Ativo);
        return _mapper.Map<IEnumerable<TDto>>(entities);
    }

    public virtual async Task<TDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? default : _mapper.Map<TDto>(entity);
    }

    public virtual async Task<TDto> CreateAsync(TCreateDto createDto)
    {
        // Validações específicas devem ser implementadas nas classes derivadas
        await ValidateCreateAsync(createDto);
        
        var entity = _mapper.Map<TEntity>(createDto);
        entity.DataCriacao = DateTime.UtcNow;
        entity.Ativo = true;
        
        var createdEntity = await _repository.AddAsync(entity);
        return _mapper.Map<TDto>(createdEntity);
    }

    public virtual async Task<TDto> UpdateAsync(int id, TUpdateDto updateDto)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            throw new ArgumentException($"Registro com ID {id} não encontrado");
        
        // Validações específicas devem ser implementadas nas classes derivadas
        await ValidateUpdateAsync(id, updateDto);
        
        _mapper.Map(updateDto, entity);
        entity.DataAlteracao = DateTime.UtcNow;
        
        var updatedEntity = await _repository.UpdateAsync(entity);
        return _mapper.Map<TDto>(updatedEntity);
    }

    public virtual async Task<bool> ActivateAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return false;
        
        entity.Ativo = true;
        entity.DataAlteracao = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);
        return true;
    }

    public virtual async Task<bool> DeactivateAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity == null)
            return false;
        
        entity.Ativo = false;
        entity.DataAlteracao = DateTime.UtcNow;
        await _repository.UpdateAsync(entity);
        return true;
    }

    public virtual async Task<bool> DeleteAsync(int id)
    {
        return await _repository.DeleteAsync(id);
    }

    /// <summary>
    /// Método para validações customizadas na criação
    /// </summary>
    protected virtual Task ValidateCreateAsync(TCreateDto createDto)
    {
        return Task.CompletedTask;
    }

    /// <summary>
    /// Método para validações customizadas na atualização
    /// </summary>
    protected virtual Task ValidateUpdateAsync(int id, TUpdateDto updateDto)
    {
        return Task.CompletedTask;
    }
}
