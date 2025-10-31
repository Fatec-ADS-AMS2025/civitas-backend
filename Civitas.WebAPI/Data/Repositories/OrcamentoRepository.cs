using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Repositories
{
    public class OrcamentoRepository : GenericRepository<Orcamento>, IOrcamentoRepository
    {
        private readonly AppDbContext _context;

        public OrcamentoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
