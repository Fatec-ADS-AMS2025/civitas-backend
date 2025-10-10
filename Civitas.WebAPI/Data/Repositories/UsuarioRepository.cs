using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IEnumerable<Usuario>> GetUsuarioByCpf(string cpf)
        {
            return await _context.Usuarios.Where(m => m.Cpf.Contains(cpf)).ToListAsync();
        }

    }
}
