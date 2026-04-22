namespace Civitas.WebAPI.Services.Validation
{
    public class OrcamentoValidationException : Exception
    {
        public OrcamentoValidationException(IEnumerable<string> errors)
            : base("Os dados informados para o orcamento sao invalidos.")
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
