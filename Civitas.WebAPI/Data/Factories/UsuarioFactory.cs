using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>
    /// Factory de <see cref="Usuario"/>. Senha padrão: <see cref="DefaultPassword"/>.
    /// </summary>
    public class UsuarioFactory : Factory<Usuario, UsuarioFactory>
    {
        /// <summary>Senha em texto puro de todos os usuários gerados.</summary>
        public const string DefaultPassword = "password";

        // BCrypt é caro de propósito; calcula o hash uma única vez.
        private static readonly Lazy<string> DefaultPasswordHash = new(() => BCrypt.Net.BCrypt.HashPassword(DefaultPassword));

        protected override Usuario Definition()
        {
            var sequence = NextSequence();
            var nome = Faker.Name.FullName();

            return new Usuario(
                0,
                Cpf(),
                nome,
                Digits(9),
                Faker.Address.StreetName(),
                Faker.Random.Int(1, 9999).ToString(),
                Faker.Address.City(),
                Uf(),
                Digits(8),
                $"usuario{sequence}@civitas.dev",
                DefaultPasswordHash.Value,
                Situacao.ATIVO,
                $"MAT-{sequence:D5}",
                TipoUsuario.FUNCIONARIO,
                Faker.Address.County());
        }

        public UsuarioFactory Administrador() => State(u => u.TipoUsuario = TipoUsuario.ADMINISTRADOR);

        public UsuarioFactory Funcionario() => State(u => u.TipoUsuario = TipoUsuario.FUNCIONARIO);

        public UsuarioFactory Visitante() => State(u => u.TipoUsuario = TipoUsuario.VISITANTE);

        public UsuarioFactory Inativo() => State(u => u.Situacao = Situacao.INATIVO);
    }
}
