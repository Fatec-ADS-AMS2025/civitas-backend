using Civitas.WebAPI.Services.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Civitas.WebAPI.Hubs;

/// <summary>
/// Canal de leitura em tempo real para notificações de novas auditorias.
/// Clientes não podem invocar operações para criar ou alterar registros.
/// </summary>
[Authorize(Policy = AuditoriaAuthorization.PolicyName)]
public sealed class AuditoriaHub : Hub
{
    /// <summary>Nome público do evento enviado aos clientes autorizados.</summary>
    public const string EventoCriada = "AuditoriaCriada";
}
