namespace Civitas.WebAPI.Services.Validation
{
    public class UnidadeConsumidoraValidationException : Exception
    {
        public UnidadeConsumidoraValidationException(IEnumerable<string> errors)
            : base("Os dados informados para a unidade consumidora sao invalidos.")
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
