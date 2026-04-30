namespace Civitas.WebAPI.Services.Validation
{
    public class UnidadeConsumidoraConflictException : Exception
    {
        public UnidadeConsumidoraConflictException(string? field, string message) : base(message)
        {
            Field = field;
        }

        public string? Field { get; }
    }
}
