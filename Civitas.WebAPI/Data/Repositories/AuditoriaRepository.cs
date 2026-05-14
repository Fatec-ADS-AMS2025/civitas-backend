using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class AuditoriaRepository : GenericRepository<Auditoria>, IAuditoriaRepository
    {
        private readonly AppDbContext _context;

        public AuditoriaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Auditoria>> GetByUsuarioId(int usuarioId)
        {
            return await _context.Auditorias
                .Include(a => a.Usuario)
                .Where(a => a.UsuarioId == usuarioId && !a.Excluido)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auditoria>> GetByEntidade(string nomeEntidade)
        {
            return await _context.Auditorias
                .Include(a => a.Usuario)
                .Where(a => a.NomeEntidade.Contains(nomeEntidade) && !a.Excluido)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Auditoria>> GetByOperacao(string operacao)
        {
            return await _context.Auditorias
                .Include(a => a.Usuario)
                .Where(a => a.Operacao.Contains(operacao) && !a.Excluido)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }
    }
}
