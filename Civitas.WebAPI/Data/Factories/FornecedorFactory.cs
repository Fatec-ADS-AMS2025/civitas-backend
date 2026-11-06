using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>Factory de <see cref="Fornecedor"/>. Cria o <see cref="TipoDespesa"/> quando não informado.</summary>
    public class FornecedorFactory : Factory<Fornecedor, FornecedorFactory>
    {
        protected override Fornecedor Definition()
        {
            var sequence = NextSequence();
            var empresa = Faker.Company.CompanyName();

            return new Fornecedor(
                0,
                empresa,
                Situacao.ATIVO,
                Cnpj(),
                $"{empresa} LTDA",
                Faker.Address.StreetName(),
                Faker.Random.Int(1, 9999).ToString(),
                Faker.Address.County(),
                Digits(8),
                Telefone(),
                $"fornecedor{sequence}@civitas.dev",
                Faker.Address.City(),
                Uf(),
                0)
            {
                TipoDespesa = TipoDespesaFactory.New().Make()
            };
        }

        public FornecedorFactory ParaTipoDespesa(TipoDespesa tipoDespesa) => State(f =>
        {
            f.TipoDespesa = tipoDespesa;
            f.IdTipoDespesa = tipoDespesa.Id;
        });

        public FornecedorFactory Inativo() => State(f => f.Situacao = Situacao.INATIVO);
    }
}
