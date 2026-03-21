namespace Civitas.WebAPI.Services.Validation
{
    public class UsuarioValidationException : Exception
    {
        public UsuarioValidationException(IEnumerable<string> errors)
            : base("Os dados informados para o usuário são inválidos.")
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
