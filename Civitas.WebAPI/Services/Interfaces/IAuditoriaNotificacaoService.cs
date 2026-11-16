using Civitas.WebAPI.Objects.Dtos.Entities;

namespace Civitas.WebAPI.Services.Interfaces;

/// <summary>Publica notificações enxutas de auditorias já persistidas.</summary>
public interface IAuditoriaNotificacaoService
{
    /// <summary>Envia o evento de criação aos clientes conectados autorizados.</summary>
    Task PublicarCriadaAsync(AuditoriaNotificacaoDTO notificacao);
}
