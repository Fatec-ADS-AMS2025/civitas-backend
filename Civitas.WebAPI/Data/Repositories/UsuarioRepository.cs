using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        private readonly AppDbContext _appDbContext;

        public UsuarioRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }
    }
}
