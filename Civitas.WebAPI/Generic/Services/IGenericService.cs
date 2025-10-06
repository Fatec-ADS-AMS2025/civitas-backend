using System.Linq.Expressions;

namespace Civitas.WebAPI.Generic.Services;

/// <summary>
/// Interface genérica para serviços com operações CRUD básicas
/// </summary>
/// <typeparam name="TDto">Tipo do DTO de resposta</typeparam>
/// <typeparam name="TCreateDto">Tipo do DTO de criação</typeparam>
/// <typeparam name="TUpdateDto">Tipo do DTO de atualização</typeparam>
public interface IGenericService<TDto, TCreateDto, TUpdateDto>
{
    /// <summary>
    /// Busca todos os registros
    /// </summary>
    Task<IEnumerable<TDto>> GetAllAsync();
    
    /// <summary>
    /// Busca apenas registros ativos
    /// </summary>
    Task<IEnumerable<TDto>> GetActiveAsync();
    
    /// <summary>
    /// Busca registro por ID
    /// </summary>
    Task<TDto?> GetByIdAsync(int id);
    
    /// <summary>
    /// Cria um novo registro
    /// </summary>
    Task<TDto> CreateAsync(TCreateDto createDto);
    
    /// <summary>
    /// Atualiza um registro existente
    /// </summary>
    Task<TDto> UpdateAsync(int id, TUpdateDto updateDto);
    
    /// <summary>
    /// Ativa um registro (soft delete)
    /// </summary>
    Task<bool> ActivateAsync(int id);
    
    /// <summary>
    /// Desativa um registro (soft delete)
    /// </summary>
    Task<bool> DeactivateAsync(int id);
    
    /// <summary>
    /// Remove permanentemente um registro
    /// </summary>
    Task<bool> DeleteAsync(int id);
}
