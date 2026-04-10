using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Repositories
{
    public class TipoCodigoRepository : GenericRepository<TipoCodigo>, ITipoCodigoRepository
    {
        private readonly AppDbContext _context;

        public TipoCodigoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}