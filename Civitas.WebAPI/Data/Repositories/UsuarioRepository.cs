using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Enums;
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

        public async Task<IEnumerable<Usuario>> GetUsuarioByCpf(string cpf)
        {
            return await _context.Usuarios.Where(m => m.Cpf.Contains(cpf)).ToListAsync();
        }

        public async Task UpdateSituacao(int id, Situacao situacao)
        {
            if (!Enum.IsDefined(typeof(Situacao), situacao))
            {
                throw new ArgumentOutOfRangeException(nameof(situacao), $"Valor inválido: {situacao} apenas é aceito 1 (ativo) e 2 (negativo)");
            }

            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
            {
                throw new KeyNotFoundException($"Usuário com id {id} não foi encontrado.");
            }

            usuario.Situacao = situacao;

            _context.Entry(usuario).Property(u => u.Situacao).IsModified = true;

            await _context.SaveChangesAsync();
        }
    }
}
