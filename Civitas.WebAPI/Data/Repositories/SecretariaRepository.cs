using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Repositories
{
    public class SecretariaRepository : GenericRepository<Secretaria>, ISecretariaRepository
    {
        private readonly AppDbContext _appDbContext;

        public SecretariaRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }
    }
}
