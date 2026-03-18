namespace Civitas.WebAPI.Services.Validation
{
    public class UsuarioConflictException : Exception
    {
        public UsuarioConflictException(string field, string message)
            : base(message)
        {
            Field = field;
        }

        public string Field { get; }
    }
}
