using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>
    /// Factory de <see cref="TipoDespesa"/>. Cria <see cref="TipoCodigo"/> e <see cref="UnidadeMedida"/>
    /// automaticamente quando não informados.
    /// </summary>
    public class TipoDespesaFactory : Factory<TipoDespesa, TipoDespesaFactory>
    {
        private static readonly string[] Descricoes =
            ["Energia Eletrica", "Agua e Esgoto", "Internet", "Telefonia", "Manutencao Predial", "Limpeza", "Combustivel"];

        protected override TipoDespesa Definition()
        {
            return new TipoDespesa(0, Faker.PickRandom(Descricoes), Faker.PickRandom<SolicitaUc>(), Situacao.ATIVO)
            {
                TipoCodigo = TipoCodigoFactory.New().Make(),
                UnidadeMedida = UnidadeMedidaFactory.New().Make()
            };
        }

        public TipoDespesaFactory ParaTipoCodigo(TipoCodigo tipoCodigo) => State(t =>
        {
            t.TipoCodigo = tipoCodigo;
            t.IdTipoCodigo = tipoCodigo.Id;
        });

        public TipoDespesaFactory ParaUnidadeMedida(UnidadeMedida unidadeMedida) => State(t =>
        {
            t.UnidadeMedida = unidadeMedida;
            t.IdUnidadeMedida = unidadeMedida.Id;
        });

        public TipoDespesaFactory SolicitaUc() => State(t => t.SolicitaUc = Objects.Enums.SolicitaUc.Sim);

        public TipoDespesaFactory NaoSolicitaUc() => State(t => t.SolicitaUc = Objects.Enums.SolicitaUc.Não);

        /// <summary>Declara campos opcionais no envelope <c>{"camposOpcionais": [...]}</c>.</summary>
        public TipoDespesaFactory ComCamposOpcionais(params string[] campos) => State(t =>
            t.CamposOpcionais = System.Text.Json.JsonSerializer.Serialize(new { camposOpcionais = campos }));

        public TipoDespesaFactory Inativo() => State(t => t.Situacao = Situacao.INATIVO);
    }
}
