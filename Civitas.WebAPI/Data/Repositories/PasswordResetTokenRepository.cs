using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private readonly AppDbContext _context;

        public PasswordResetTokenRepository(AppDbContext context) => _context = context;

        public async Task AddAsync(PasswordResetToken token)
        {
            await _context.PasswordResetTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task InvalidateUnusedForUserAsync(int usuarioId, DateTime usedAt)
        {
            await _context.PasswordResetTokens
                .Where(token => token.UsuarioId == usuarioId && token.UtilizadoEm == null)
                .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UtilizadoEm, usedAt));
        }

        public Task<PasswordResetToken?> GetActiveByHashAsync(string tokenHash, DateTime now) =>
            _context.PasswordResetTokens
                .AsNoTracking()
                .FirstOrDefaultAsync(token => token.TokenHash == tokenHash && token.UtilizadoEm == null && token.ExpiraEm > now);

        public async Task<bool> TryConsumeAsync(int tokenId, DateTime usedAt)
        {
            var affected = await _context.PasswordResetTokens
                .Where(token => token.Id == tokenId && token.UtilizadoEm == null && token.ExpiraEm > usedAt)
                .ExecuteUpdateAsync(setters => setters.SetProperty(token => token.UtilizadoEm, usedAt));
            return affected == 1;
        }
    }
}
