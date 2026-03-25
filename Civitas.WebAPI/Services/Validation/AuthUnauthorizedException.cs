namespace Civitas.WebAPI.Services.Validation
{
    public class AuthUnauthorizedException : Exception
    {
        public AuthUnauthorizedException(string message) : base(message)
        {
        }
    }
}
