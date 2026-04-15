namespace Civitas.WebAPI.Services.Validation
{
    public class InstituicaoConflictException : Exception
    {
        public InstituicaoConflictException(string field, string message)
            : base(message)
        {
            Field = field;
        }

        public string Field { get; }
    }
}
