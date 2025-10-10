using Civitas.WebAPI.Data.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;

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
            return await _dbSet.ToListAsync();
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
<<<<<<< HEAD
            // Descobre dinamicamente o nome da chave primária
            var keyName = _context.Model.FindEntityType(typeof(T))!
                .FindPrimaryKey()!
                .Properties
                .Select(x => x.Name)
                .First();

            // Pega o valor da chave primária da entidade atual
            var entityId = _context.Entry(entity).Property(keyName).CurrentValue;

            // Verifica se a entidade com o mesmo Id já está sendo rastreada
            var trackedEntity = _context.ChangeTracker.Entries<T>()
                .FirstOrDefault(e => e.Property(keyName).CurrentValue.Equals(entityId));

            // Se a entidade já estiver sendo rastreada, desanexa
            if (trackedEntity != null)
                _context.Entry(trackedEntity.Entity).State = EntityState.Detached;

            // Marca como modificada
            _context.Entry(entity).State = EntityState.Modified;

            await SaveChanges();
=======
          // Descobre dinamicamente o nome da chave primária
          var keyName = _context.Model.FindEntityType(typeof(T))!
            .FindPrimaryKey()!
            .Properties
            .Select(x => x.Name)
            .First();

          // Pega o valor da chave primária da entidade atual
           var entityId = _context.Entry(entity).Property(keyName).CurrentValue;

          // Verifica se a entidade com o mesmo Id já está sendo rastreada
          var trackedEntity = _context.ChangeTracker.Entries<T>()
          .FirstOrDefault(e => e.Property(keyName).CurrentValue.Equals(entityId));

          // Se a entidade já estiver sendo rastreada, desanexa
          if (trackedEntity != null)
            _context.Entry(trackedEntity.Entity).State = EntityState.Detached;

          // Marca como modificada
          _context.Entry(entity).State = EntityState.Modified;

          await SaveChanges();
>>>>>>> 8a9f7bc6ddcb66105d5aac725e02c1a58029f95d
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
    }
}
