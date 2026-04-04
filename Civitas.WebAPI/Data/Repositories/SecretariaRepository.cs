using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class SecretariaRepository : GenericRepository<Secretaria>, ISecretariaRepository
    {
        private readonly AppDbContext _appDbContext;

        public SecretariaRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<bool> ExistsByCnpjAsync(string cnpj, int? ignoreId = null)
        {
            var query = _appDbContext.Secretarias
                .AsNoTracking()
                .Where(secretaria => secretaria.Cnpj == cnpj);

            if (ignoreId.HasValue)
            {
                query = query.Where(secretaria => secretaria.IdSecretaria != ignoreId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? ignoreId = null)
        {
            var normalizedEmail = email.ToLower();

            var query = _appDbContext.Secretarias
                .AsNoTracking()
                .Where(secretaria => secretaria.Email.ToLower() == normalizedEmail);

            if (ignoreId.HasValue)
            {
                query = query.Where(secretaria => secretaria.IdSecretaria != ignoreId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
