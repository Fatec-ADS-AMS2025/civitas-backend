namespace Civitas.WebAPI.Services.Security
{
    public interface IPasswordHashService
    {
        string Hash(string password);
        bool Verify(string password, string hash);
    }
}
