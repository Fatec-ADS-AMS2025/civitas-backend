using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Repositories
{
    public class FluxoRepository : GenericRepository<Fluxo>, IFluxoRepository
    {
        private readonly AppDbContext _appDbContext;

        public FluxoRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }
    }
}
