using Civitas.WebAPI.Hubs;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace Civitas.WebAPI.Services.Entities;

/// <summary>Transmite avisos de auditoria sem acoplar o fluxo de persistência ao Hub.</summary>
public sealed class AuditoriaNotificacaoService : IAuditoriaNotificacaoService
{
    private readonly IHubContext<AuditoriaHub> _hubContext;
    private readonly ILogger<AuditoriaNotificacaoService> _logger;

    public AuditoriaNotificacaoService(
        IHubContext<AuditoriaHub> hubContext,
        ILogger<AuditoriaNotificacaoService> logger)
    {
        _hubContext = hubContext;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task PublicarCriadaAsync(AuditoriaNotificacaoDTO notificacao)
    {
        try
        {
            await _hubContext.Clients.All.SendAsync(AuditoriaHub.EventoCriada, notificacao);
        }
        catch (Exception exception)
        {
            // A persistência já foi confirmada. Uma falha transitória do transporte
            // não pode converter o sucesso da operação em falha nem expor o payload.
            _logger.LogWarning(
                "Falha técnica ao publicar a notificação da auditoria {AuditoriaId}. Tipo: {ExceptionType}.",
                notificacao.Id,
                exception.GetType().Name);
        }
    }
}
