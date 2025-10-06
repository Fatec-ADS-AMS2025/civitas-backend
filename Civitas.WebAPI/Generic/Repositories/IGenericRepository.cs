using System.Linq.Expressions;

namespace Civitas.WebAPI.Generic.Repositories;

/// <summary>
/// Interface genérica para repositórios com operações CRUD básicas
/// </summary>
/// <typeparam name="TEntity">Tipo da entidade</typeparam>
public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Busca todas as entidades
    /// </summary>
    Task<IEnumerable<TEntity>> GetAllAsync();
    
    /// <summary>
    /// Busca entidades com filtro
    /// </summary>
    Task<IEnumerable<TEntity>> GetWhereAsync(Expression<Func<TEntity, bool>> predicate);
    
    /// <summary>
    /// Busca entidade por ID
    /// </summary>
    Task<TEntity?> GetByIdAsync(int id);
    
    /// <summary>
    /// Busca primeira entidade que atende o filtro
    /// </summary>
    Task<TEntity?> GetFirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);
    
    /// <summary>
    /// Verifica se existe alguma entidade que atende o filtro
    /// </summary>
    Task<bool> ExistsAsync(Expression<Func<TEntity, bool>> predicate);
    
    /// <summary>
    /// Adiciona uma nova entidade
    /// </summary>
    Task<TEntity> AddAsync(TEntity entity);
    
    /// <summary>
    /// Atualiza uma entidade existente
    /// </summary>
    Task<TEntity> UpdateAsync(TEntity entity);
    
    /// <summary>
    /// Remove uma entidade do banco de dados
    /// </summary>
    Task<bool> DeleteAsync(int id);
    
    /// <summary>
    /// Salva as alterações no contexto
    /// </summary>
    Task<int> SaveChangesAsync();
}
