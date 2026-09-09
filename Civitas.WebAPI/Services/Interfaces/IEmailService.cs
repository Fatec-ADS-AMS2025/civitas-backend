using Civitas.WebAPI.Objects.Dtos.Entities;

namespace Civitas.WebAPI.Services.Interfaces
{
    public interface IEmailService
    {
        Task<EmailResponseDto> EnviarAsync(EmailEnvioDto dto);

        Task<IReadOnlyList<EmailResponseDto>> ListarAsync(EmailFiltroDto? filtro = null);

        Task<EmailResponseDto> ObterPorIdAsync(int id);

        Task<IReadOnlyList<EmailResponseDto>> ListarPorDestinatarioAsync(string email);

        Task<IReadOnlyList<EmailResponseDto>> ListarPorStatusAsync(string status);
    }
}
