using Civitas.WebAPI.Objects.Dtos.Entities;

namespace Civitas.WebAPI.Services.Validation
{
    public class EmailSendException : Exception
    {
        public EmailSendException(string message, EmailResponseDto email, Exception? innerException = null)
            : base(message, innerException)
        {
            Email = email;
        }

        public EmailResponseDto Email { get; }
    }
}
