namespace Civitas.WebAPI.Services.Validation
{
    public class DespesaValidationException : Exception
    {
        public DespesaValidationException(IEnumerable<string> errors)
            : base("Os dados informados para a despesa sao invalidos.")
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
