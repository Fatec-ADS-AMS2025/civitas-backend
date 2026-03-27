using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class UsuarioRepository : GenericRepository<Usuario>, IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByCpf(string cpf, int? ignoredId = null)
        {
            return await _context.Usuarios.AnyAsync(usuario =>
                usuario.Cpf == cpf &&
                (!ignoredId.HasValue || usuario.Id != ignoredId.Value));
        }

        public async Task<bool> ExistsByEmail(string email, int? ignoredId = null)
        {
            return await _context.Usuarios.AnyAsync(usuario =>
                usuario.Email == email &&
                (!ignoredId.HasValue || usuario.Id != ignoredId.Value));
        }

        public async Task<bool> ExistsByMatricula(string matricula, int? ignoredId = null)
        {
            return await _context.Usuarios.AnyAsync(usuario =>
                usuario.Matricula == matricula &&
                (!ignoredId.HasValue || usuario.Id != ignoredId.Value));
        }

        public async Task<Usuario?> GetByEmailAsync(string email)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(usuario => usuario.Email == email);
        }

        public async Task<IEnumerable<Usuario>> GetUsuarioByCpf(string cpf)
        {
            return await _context.Usuarios
                .Where(usuario => usuario.Cpf == cpf)
                .ToListAsync();
        }
    }
}
