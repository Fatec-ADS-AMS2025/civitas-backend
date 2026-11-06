using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>
    /// Factory de <see cref="Instituicao"/>. Cria <see cref="Secretaria"/> e <see cref="TipoInstituicao"/>
    /// quando não informados.
    /// </summary>
    public class InstituicaoFactory : Factory<Instituicao, InstituicaoFactory>
    {
        protected override Instituicao Definition()
        {
            var sequence = NextSequence();
            var nome = $"Instituicao {Faker.Address.County()} {sequence}";

            return new Instituicao(
                0,
                Cnpj(),
                nome,
                Faker.Address.StreetName(),
                Faker.Random.Int(1, 9999).ToString(),
                Faker.Address.County(),
                Digits(8),
                nome,
                Telefone(),
                $"instituicao{sequence}@civitas.dev",
                Faker.Address.City(),
                Uf(),
                Situacao.ATIVO)
            {
                Secretaria = SecretariaFactory.New().Make(),
                TipoInstituicao = TipoInstituicaoFactory.New().Make()
            };
        }

        public InstituicaoFactory ParaSecretaria(Secretaria secretaria) => State(i =>
        {
            i.Secretaria = secretaria;
            i.IdSecretaria = secretaria.IdSecretaria;
        });

        public InstituicaoFactory ParaTipoInstituicao(TipoInstituicao tipoInstituicao) => State(i =>
        {
            i.TipoInstituicao = tipoInstituicao;
            i.IdTipoInstituicao = tipoInstituicao.Id;
        });

        public InstituicaoFactory Inativa() => State(i => i.Situacao = Situacao.INATIVO);
    }
}
