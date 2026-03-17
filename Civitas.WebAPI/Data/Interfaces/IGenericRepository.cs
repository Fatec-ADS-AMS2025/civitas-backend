using Civitas.WebAPI.Objects.Contracts;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> Get();
        Task<PaginatedResult<T>> GetPage(PaginationQuery paginationQuery);
        Task<T> GetById(int id);
        Task Add(T entity);
        Task Update(T entity);
        Task Remove(T entity);
        Task<int> SaveChanges();
    }
}
