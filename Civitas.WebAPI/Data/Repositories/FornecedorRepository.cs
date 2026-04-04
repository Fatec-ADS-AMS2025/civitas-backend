using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class FornecedorRepository : GenericRepository<Fornecedor>, IFornecedorRepository
    {
        private readonly AppDbContext _appDbContext;

        public FornecedorRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<bool> ExistsByCnpjAsync(string cnpj, int? ignoreId = null)
        {
            var query = _appDbContext.Fornecedores
                .AsNoTracking()
                .Where(fornecedor => fornecedor.Cnpj == cnpj);

            if (ignoreId.HasValue)
            {
                query = query.Where(fornecedor => fornecedor.IdFornecedor != ignoreId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
