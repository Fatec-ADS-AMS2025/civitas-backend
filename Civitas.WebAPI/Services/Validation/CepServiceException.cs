namespace Civitas.WebAPI.Services.Validation
{
    /// <summary>
    /// Exceção lançada quando ocorre falha de infraestrutura na consulta externa de CEP.
    /// </summary>
    public class CepServiceException : Exception
    {
        public CepServiceException(string message, Exception? innerException = null)
            : base(message, innerException)
        {
        }
    }
}
