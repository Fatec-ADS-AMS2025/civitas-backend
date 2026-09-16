using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IPasswordResetTokenRepository
    {
        Task AddAsync(PasswordResetToken token);
        Task InvalidateUnusedForUserAsync(int usuarioId, DateTime usedAt);
        Task<PasswordResetToken?> GetActiveByHashAsync(string tokenHash, DateTime now);
        Task<bool> TryConsumeAsync(int tokenId, DateTime usedAt);
    }
}
