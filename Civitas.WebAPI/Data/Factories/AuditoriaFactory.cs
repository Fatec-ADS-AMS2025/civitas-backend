using System.Text.Json;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>Factory de <see cref="Auditoria"/>. Cria o <see cref="Usuario"/> quando não informado.</summary>
    public class AuditoriaFactory : Factory<Auditoria, AuditoriaFactory>
    {
        private static readonly string[] Entidades = ["Despesa", "Fornecedor", "Instituicao", "Orcamento", "Secretaria", "Usuario"];

        protected override Auditoria Definition()
        {
            var momento = Faker.Date.Recent(30).ToUniversalTime();

            return new Auditoria
            {
                Data = momento.ToString("yyyy-MM-dd"),
                Hora = momento.ToString("HH:mm:ss"),
                Operacao = "INSERT",
                NomeEntidade = Faker.PickRandom(Entidades),
                ValoresAtingidos = "{}",
                NovosValores = JsonSerializer.Serialize(new { id = Faker.Random.Int(1, 1000) }),
                Situacao = Situacao.ATIVO,
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
            a.Operacao = "INSERT";
            a.NomeEntidade = entidade;
            a.ValoresAtingidos = "{}";
            a.NovosValores = JsonSerializer.Serialize(novosValores);
        });

        public AuditoriaFactory Atualizacao(string entidade, object valoresAntigos, object novosValores) => State(a =>
        {
            a.Operacao = "UPDATE";
            a.NomeEntidade = entidade;
            a.ValoresAtingidos = JsonSerializer.Serialize(valoresAntigos);
            a.NovosValores = JsonSerializer.Serialize(novosValores);
        });

        public AuditoriaFactory Exclusao(string entidade, object valoresAntigos) => State(a =>
        {
            a.Operacao = "DELETE";
            a.NomeEntidade = entidade;
            a.ValoresAtingidos = JsonSerializer.Serialize(valoresAntigos);
            a.NovosValores = "{}";
        });
    }
}
