using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>Factory de <see cref="Secretaria"/>.</summary>
    public class SecretariaFactory : Factory<Secretaria, SecretariaFactory>
    {
        private static readonly string[] Areas =
            ["Educacao", "Saude", "Obras", "Meio Ambiente", "Cultura", "Esporte", "Assistencia Social", "Transporte"];

        protected override Secretaria Definition()
        {
            var sequence = NextSequence();
            var area = Faker.PickRandom(Areas);
            var nome = $"Secretaria Municipal de {area}";

            return new Secretaria(
                0,
                Situacao.ATIVO,
                $"Responsavel pela gestao da area de {area.ToLowerInvariant()}.",
                Cnpj(),
                nome,
                Faker.Address.StreetName(),
                Faker.Random.Int(1, 9999).ToString(),
                Faker.Address.County(),
                Digits(8),
                $"{nome} {Faker.Address.City()}",
                $"secretaria{sequence}@civitas.dev",
                Telefone(),
                Faker.Address.City(),
                Uf());
        }

        public SecretariaFactory Inativa() => State(s => s.Situacao = Situacao.INATIVO);
    }
}
