namespace Civitas.WebAPI.Objects.Dtos.Entities;

/// <summary>
/// Payload enxuto para avisar que um registro de auditoria foi persistido.
/// Snapshots e dados do usuário permanecem disponíveis somente pela API protegida.
/// </summary>
public sealed class AuditoriaNotificacaoDTO
{
    /// <summary>Identificador persistido do registro.</summary>
    public int Id { get; init; }

    /// <summary>Operação registrada, como INSERT, UPDATE ou DELETE.</summary>
    public string Operacao { get; init; } = string.Empty;

    /// <summary>Nome da entidade afetada.</summary>
    public string NomeEntidade { get; init; } = string.Empty;

    /// <summary>Data original armazenada no registro de auditoria.</summary>
    public string Data { get; init; } = string.Empty;

    /// <summary>Hora original armazenada no registro de auditoria.</summary>
    public string Hora { get; init; } = string.Empty;

    /// <summary>Instante UTC em que a notificação foi publicada, em ISO 8601.</summary>
    public DateTimeOffset NotificadoEmUtc { get; init; }
}
