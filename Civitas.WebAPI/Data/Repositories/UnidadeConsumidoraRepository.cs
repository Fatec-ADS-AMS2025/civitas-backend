using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Repositories
{
    public class UnidadeConsumidoraRepository : GenericRepository<UnidadeConsumidora>, IUnidadeConsumidoraRepository
    {
        public UnidadeConsumidoraRepository(AppDbContext context) : base(context)
        {
        }
    }
}
