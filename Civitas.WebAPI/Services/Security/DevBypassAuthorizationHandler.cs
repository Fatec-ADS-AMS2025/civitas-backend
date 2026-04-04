using Microsoft.AspNetCore.Authorization;

namespace Civitas.WebAPI.Services.Security
{
    /// <summary>
    /// Handler de autorização que ignora todos os requisitos de segurança quando o modo DEV está ativo.
    /// Controlado pela propriedade <c>DEV</c> em <c>appsettings.json</c>.
    /// Quando <c>DEV = true</c>, todas as rotas protegidas ficam acessíveis sem token.
    /// Quando <c>DEV = false</c>, a validação JWT normal é aplicada.
    /// </summary>
    public class DevBypassAuthorizationHandler : IAuthorizationHandler
    {
        private readonly bool _devMode;

        public DevBypassAuthorizationHandler(IConfiguration configuration)
        {
            _devMode = configuration.GetValue<bool>("DEV");
        }

        /// <inheritdoc />
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            if (_devMode)
            {
                foreach (var requirement in context.PendingRequirements.ToList())
                    context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
