namespace Civitas.WebAPI.Services.Validation
{
    public class TipoDespesaValidationException : Exception
    {
        public TipoDespesaValidationException(IEnumerable<string> errors)
            : base("Os dados informados para o tipo de despesa são inválidos.")
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
