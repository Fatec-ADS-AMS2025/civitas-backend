using System.Globalization;
using System.Text.Json;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Validation;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>
    /// Factory de <see cref="Auditoria"/>. Cria o <see cref="Usuario"/> quando não informado.
    /// </summary>
    /// <remarks>
    /// Segue a convenção validada pelo <see cref="AuditoriaValidator"/>: snapshot vazio é string vazia,
    /// INSERT só tem NovosValores, DELETE só tem ValoresAtingidos.
    /// </remarks>
    public class AuditoriaFactory : Factory<Auditoria, AuditoriaFactory>
    {
        private static readonly string[] Entidades = ["Despesa", "Fornecedor", "Instituicao", "Orcamento", "Secretaria", "Usuario"];

        protected override Auditoria Definition()
        {
            var momento = Faker.Date.Recent(30).ToUniversalTime();

            return new Auditoria
            {
                Data = momento.ToString(AuditoriaValidator.FormatoData, CultureInfo.InvariantCulture),
                Hora = momento.ToString(AuditoriaValidator.FormatoHora, CultureInfo.InvariantCulture),
                Operacao = AuditoriaOperacao.INSERT,
                NomeEntidade = Faker.PickRandom(Entidades),
                ValoresAtingidos = string.Empty,
                NovosValores = JsonSerializer.Serialize(new { id = Faker.Random.Int(1, 1000) }),
                Usuario = UsuarioFactory.New().Make()
            };
        }

        public AuditoriaFactory ParaUsuario(Usuario usuario) => State(a =>
        {
            a.Usuario = usuario;
            a.UsuarioId = usuario.Id;
        });

        public AuditoriaFactory Insercao(string entidade, object novosValores) => State(a =>
        {
            a.Operacao = AuditoriaOperacao.INSERT;
            a.NomeEntidade = entidade;
            a.ValoresAtingidos = string.Empty;
            a.NovosValores = JsonSerializer.Serialize(novosValores);
        });

        public AuditoriaFactory Atualizacao(string entidade, object valoresAntigos, object novosValores) => State(a =>
        {
            a.Operacao = AuditoriaOperacao.UPDATE;
            a.NomeEntidade = entidade;
            a.ValoresAtingidos = JsonSerializer.Serialize(valoresAntigos);
            a.NovosValores = JsonSerializer.Serialize(novosValores);
        });

        public AuditoriaFactory Exclusao(string entidade, object valoresAntigos) => State(a =>
        {
            a.Operacao = AuditoriaOperacao.DELETE;
            a.NomeEntidade = entidade;
            a.ValoresAtingidos = JsonSerializer.Serialize(valoresAntigos);
            a.NovosValores = string.Empty;
        });
    }
}
