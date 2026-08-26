namespace Civitas.WebAPI.Services.Validation
{
    public class TipoInstituicaoValidationException : Exception
    {
        public TipoInstituicaoValidationException(IEnumerable<string> errors)
            : base("Os dados informados para o tipo de instituição são inválidos.")
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
