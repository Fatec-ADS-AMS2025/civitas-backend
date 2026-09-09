namespace Civitas.WebAPI.Services.Validation
{
    public class EmailValidationException : Exception
    {
        public EmailValidationException(IEnumerable<string> errors)
            : base("Os dados informados para o envio de e-mail são inválidos.")
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
