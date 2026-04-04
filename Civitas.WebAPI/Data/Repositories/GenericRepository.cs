using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Contracts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using System.Linq.Expressions;

namespace Civitas.WebAPI.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> Get()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public virtual async Task<PaginatedResult<T>> GetPage(PaginationQuery paginationQuery)
        {
            var currentPage = paginationQuery.NormalizedPage;
            var pageSize = paginationQuery.NormalizedSize;

            var orderedQuery = ApplyOrdering(
                _dbSet.AsNoTracking(),
                paginationQuery.SortBy,
                paginationQuery.IsDescending);

            var totalRecords = await orderedQuery.CountAsync();
            var items = await orderedQuery
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalRecords = totalRecords,
                TotalPages = totalRecords == 0
                    ? 0
                    : (int)Math.Ceiling(totalRecords / (double)pageSize),
                CurrentPage = currentPage,
                PageSize = pageSize
            };
        }

        public virtual async Task<IEnumerable<T>> GetByEnumValue<TEnum>(string propertyName, TEnum value) where TEnum : struct, Enum
        {
            var filteredQuery = ApplyEnumFilter(_dbSet.AsNoTracking(), propertyName, value);
            return await filteredQuery.ToListAsync();
        }

        public virtual async Task<PaginatedResult<T>> GetPageByEnumValue<TEnum>(PaginationQuery paginationQuery, string propertyName, TEnum value) where TEnum : struct, Enum
        {
            var currentPage = paginationQuery.NormalizedPage;
            var pageSize = paginationQuery.NormalizedSize;

            var filteredQuery = ApplyEnumFilter(_dbSet.AsNoTracking(), propertyName, value);
            var orderedQuery = ApplyOrdering(
                filteredQuery,
                paginationQuery.SortBy,
                paginationQuery.IsDescending);

            var totalRecords = await orderedQuery.CountAsync();
            var items = await orderedQuery
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<T>
            {
                Items = items,
                TotalRecords = totalRecords,
                TotalPages = totalRecords == 0
                    ? 0
                    : (int)Math.Ceiling(totalRecords / (double)pageSize),
                CurrentPage = currentPage,
                PageSize = pageSize
            };
        }

        public async Task<T> GetById(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task Add(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveChanges();
        }

        public async Task Update(T entity)
        {
            var keyName = _context.Model.FindEntityType(typeof(T))!
                .FindPrimaryKey()!
                .Properties
                .Select(x => x.Name)
                .First();

            var entityId = _context.Entry(entity).Property(keyName).CurrentValue;

            var trackedEntity = _context.ChangeTracker.Entries<T>()
                .FirstOrDefault(e => Equals(e.Property(keyName).CurrentValue, entityId));

            if (trackedEntity != null)
            {
                _context.Entry(trackedEntity.Entity).State = EntityState.Detached;
            }

            _context.Entry(entity).State = EntityState.Modified;

            await SaveChanges();
        }

        public async Task Remove(T entity)
        {
            _dbSet.Remove(entity);
            await SaveChanges();
        }

        public async Task<int> SaveChanges()
        {
            return await _context.SaveChangesAsync();
        }

        private IQueryable<T> ApplyOrdering(IQueryable<T> query, string? requestedSortBy, bool isDescending)
        {
            var entityType = _context.Model.FindEntityType(typeof(T))
                ?? throw new InvalidOperationException($"Entity type {typeof(T).Name} is not mapped.");

            var sortPropertyName = ResolveSortPropertyName(entityType, requestedSortBy);
            var parameter = Expression.Parameter(typeof(T), "entity");
            var property = Expression.Property(parameter, sortPropertyName);
            var selector = Expression.Lambda(property, parameter);
            var methodName = isDescending ? nameof(Queryable.OrderByDescending) : nameof(Queryable.OrderBy);

            var orderedQuery = typeof(Queryable)
                .GetMethods()
                .Single(method =>
                    method.Name == methodName &&
                    method.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(T), property.Type)
                .Invoke(null, [query, selector]);

            return (IQueryable<T>)orderedQuery!;
        }

        private static IQueryable<T> ApplyEnumFilter<TEnum>(IQueryable<T> query, string propertyName, TEnum value) where TEnum : struct, Enum
        {
            var resolvedPropertyName = ResolvePropertyName(propertyName);
            return query.Where(entity => EF.Property<TEnum>(entity, resolvedPropertyName).Equals(value));
        }

        private static string ResolvePropertyName(string requestedPropertyName)
        {
            var propertyInfo = typeof(T).GetProperty(
                requestedPropertyName,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);

            if (propertyInfo is null)
            {
                throw new InvalidOperationException(
                    $"Property '{requestedPropertyName}' was not found on entity type {typeof(T).Name}.");
            }

            return propertyInfo.Name;
        }

        private static string ResolveSortPropertyName(IEntityType entityType, string? requestedSortBy)
        {
            var matchedProperty = entityType
                .GetProperties()
                .FirstOrDefault(property =>
                    string.Equals(property.Name, requestedSortBy, StringComparison.OrdinalIgnoreCase));

            if (matchedProperty is not null)
            {
                return matchedProperty.Name;
            }

            var primaryKeyName = entityType.FindPrimaryKey()
                ?.Properties
                .Select(property => property.Name)
                .FirstOrDefault();

            return primaryKeyName
                ?? throw new InvalidOperationException($"Entity type {entityType.Name} does not define a primary key.");
        }
    }
}
