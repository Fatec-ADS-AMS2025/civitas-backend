namespace Civitas.WebAPI.Services.Validation
{
    public class AuthValidationException : Exception
    {
        public AuthValidationException(IEnumerable<string> errors)
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
