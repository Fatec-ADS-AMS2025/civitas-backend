using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>
    /// Factory de <see cref="Orcamento"/> (ano corrente, consumos mensais zerados).
    /// Cria <see cref="Instituicao"/> e <see cref="TipoDespesa"/> quando não informados.
    /// </summary>
    public class OrcamentoFactory : Factory<Orcamento, OrcamentoFactory>
    {
        protected override Orcamento Definition()
        {
            return new Orcamento(
                0,
                DateTime.UtcNow.Year,
                Math.Round(Faker.Random.Decimal(50_000m, 500_000m), 2),
                0,
                0,
                0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m,
                0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m, 0m)
            {
                Instituicao = InstituicaoFactory.New().Make(),
                TipoDespesa = TipoDespesaFactory.New().Make()
            };
        }

        public OrcamentoFactory ParaInstituicao(Instituicao instituicao) => State(o =>
        {
            o.Instituicao = instituicao;
            o.IdInstituicao = instituicao.Id;
        });

        public OrcamentoFactory ParaTipoDespesa(TipoDespesa tipoDespesa) => State(o =>
        {
            o.TipoDespesa = tipoDespesa;
            o.IdTipoDespesa = tipoDespesa.Id;
        });

        public OrcamentoFactory DoAno(int ano) => State(o => o.AnoOrcamento = ano);

        public OrcamentoFactory ComValor(decimal valor) => State(o => o.ValorOrcamento = valor);
    }
}
