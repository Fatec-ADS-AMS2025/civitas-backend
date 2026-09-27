namespace Civitas.WebAPI.Services.Validation
{
    public class InvalidPasswordResetTokenException : Exception
    {
        public InvalidPasswordResetTokenException() : base("Link de redefinição inválido ou expirado.") { }
    }
}
