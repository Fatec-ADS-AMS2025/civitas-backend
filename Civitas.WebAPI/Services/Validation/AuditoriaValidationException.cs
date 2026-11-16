namespace Civitas.WebAPI.Services.Validation
{
    /// <summary>Representa erros de validação agregados de um registro ou de uma consulta de auditoria.</summary>
    public class AuditoriaValidationException : Exception
    {
        public AuditoriaValidationException(IEnumerable<string> errors)
            : base("Erro na validação da auditoria.")
        {
            Errors = errors.ToArray();
        }

        public IReadOnlyCollection<string> Errors { get; }
    }
}
