namespace Civitas.WebAPI.Services.Validation
{
    public class InstituicaoValidationException : Exception
    {
        public InstituicaoValidationException(IEnumerable<string> errors)
            : base("Os dados informados para a instituicao sao invalidos.")
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
