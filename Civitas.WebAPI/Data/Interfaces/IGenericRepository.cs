using Civitas.WebAPI.Objects.Contracts;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> Get();
        Task<IEnumerable<T>> GetExcluidos();
        Task<PaginatedResult<T>> GetPage(PaginationQuery paginationQuery);
        Task<PaginatedResult<T>> GetPageExcluidos(PaginationQuery paginationQuery);
        Task<IEnumerable<T>> GetByEnumValue<TEnum>(string propertyName, TEnum value) where TEnum : struct, Enum;
        Task<PaginatedResult<T>> GetPageByEnumValue<TEnum>(PaginationQuery paginationQuery, string propertyName, TEnum value) where TEnum : struct, Enum;
        Task<T> GetById(int id);
        Task<T> GetByIdIncludingDeleted(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Remove(T entity);
        Task<int> SaveChanges();
    }
}
