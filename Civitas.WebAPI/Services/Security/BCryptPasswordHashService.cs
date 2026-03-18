using BCrypt.Net;

namespace Civitas.WebAPI.Services.Security
{
    public class BCryptPasswordHashService : IPasswordHashService
    {
        public string Hash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool Verify(string password, string hash)
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
    }
}
