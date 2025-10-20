using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class InstituicaoRepository : GenericRepository<Instituicao>, IInstituicaoRepository
    {
        private readonly AppDbContext _context;
        public InstituicaoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Instituicao>> GetInstituicaoByName(string name)
        {
            return await _context.Instituicoes.Where(m => m.Nome.Contains(name)).ToListAsync();
        }
    }
}
