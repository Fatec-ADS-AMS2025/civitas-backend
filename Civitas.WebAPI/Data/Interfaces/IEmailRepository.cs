using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IEmailRepository : IGenericRepository<EmailEnviado>
    {
        Task<IReadOnlyList<EmailEnviado>> GetByDestinatarioAsync(string emailDestinatario);

        Task<IReadOnlyList<EmailEnviado>> GetByStatusAsync(EmailStatusEnum status);
    }
}
