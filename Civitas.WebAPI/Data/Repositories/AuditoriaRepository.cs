using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    /// <summary>
    /// Acesso aos registros de auditoria. Consultas sem rastreamento; alterações e exclusões
    /// são bloqueadas pelo <see cref="AppDbContext"/> e, no PostgreSQL, por trigger.
    /// </summary>
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
                .AsNoTracking()
                .Where(a => a.UsuarioId == usuarioId)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        /// <param name="nomeEntidade">Nome já normalizado para a grafia oficial.</param>
        public async Task<IEnumerable<Auditoria>> GetByEntidade(string nomeEntidade)
        {
            return await _context.Auditorias
                .AsNoTracking()
                .Where(a => a.NomeEntidade == nomeEntidade)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        /// <param name="operacao">Operação já normalizada (INSERT, UPDATE, DELETE ou LOGIN).</param>
        public async Task<IEnumerable<Auditoria>> GetByOperacao(string operacao)
        {
            return await _context.Auditorias
                .AsNoTracking()
                .Where(a => a.Operacao == operacao)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<Auditoria?> GetByIdSemRastreamento(int id)
        {
            return await _context.Auditorias
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.Id == id);
        }
    }
}
