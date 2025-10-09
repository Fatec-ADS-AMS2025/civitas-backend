using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Repositories
{
    public class FornecedorRepository : GenericRepository<Fornecedor>, IFornecedorRepository
    {
        private readonly AppDbContext _appDbContext;

        public FornecedorRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }
    }
}
