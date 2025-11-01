using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Repositories
{
    public class UnidadeMedidaRepository : GenericRepository<UnidadeMedida>, IUnidadeMedidaRepository
    {
        private readonly AppDbContext _context;
        public UnidadeMedidaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
