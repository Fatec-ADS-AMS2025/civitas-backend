using System.Security.Cryptography;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>
    /// Factory de <see cref="PasswordResetToken"/> (padrão: token ativo, expira em 1 hora).
    /// Só existe um token ativo por usuário (índice único filtrado), então use
    /// <see cref="Utilizado"/> para gerar históricos.
    /// </summary>
    public class PasswordResetTokenFactory : Factory<PasswordResetToken, PasswordResetTokenFactory>
    {
        protected override PasswordResetToken Definition()
        {
            var criadoEm = DateTime.UtcNow;

            return new PasswordResetToken
            {
                TokenHash = Convert.ToHexStringLower(SHA256.HashData(RandomNumberGenerator.GetBytes(32))),
                CriadoEm = criadoEm,
                ExpiraEm = criadoEm.AddHours(1),
                UtilizadoEm = null,
                Usuario = UsuarioFactory.New().Make()
            };
        }

        public PasswordResetTokenFactory ParaUsuario(Usuario usuario) => State(t =>
        {
            t.Usuario = usuario;
            t.UsuarioId = usuario.Id;
        });

        public PasswordResetTokenFactory Expirado() => State(t =>
        {
            t.CriadoEm = DateTime.UtcNow.AddHours(-2);
            t.ExpiraEm = DateTime.UtcNow.AddHours(-1);
        });

        public PasswordResetTokenFactory Utilizado() => State(t => t.UtilizadoEm = t.CriadoEm.AddMinutes(10));
    }
}
