namespace Civitas.WebAPI.Services.Validation
{
    /// <summary>Representa erros de validação agregados de uma unidade de medida.</summary>
    public class UnidadeMedidaValidationException : Exception
    {
        public UnidadeMedidaValidationException(IEnumerable<string> errors)
            : base("Os dados informados para a unidade de medida são inválidos.")
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
