using Microsoft.AspNetCore.Authorization;

namespace Civitas.WebAPI.Services.Security;

/// <summary>Política exclusiva para leitura em tempo real de auditorias.</summary>
public static class AuditoriaAuthorization
{
    public const string PolicyName = "LeituraAuditoria";
}

/// <summary>Requisito que não pode ser satisfeito pelo bypass de desenvolvimento.</summary>
public sealed class AuditoriaReadRequirement : IAuthorizationRequirement
{
}

/// <summary>Permite o Hub somente a usuários autenticados com perfil administrador.</summary>
public sealed class AuditoriaReadHandler : AuthorizationHandler<AuditoriaReadRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        AuditoriaReadRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated == true &&
            context.User.HasClaim("tipousuario", "ADMINISTRADOR"))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}
