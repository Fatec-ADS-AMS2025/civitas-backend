using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>
    /// Factory de <see cref="UnidadeConsumidora"/>. Quando os relacionamentos não são informados,
    /// cria um conjunto coerente: a secretaria é a da instituição, o orçamento é da mesma
    /// instituição/tipo de despesa e o fornecedor atende o mesmo tipo de despesa.
    /// </summary>
    public class UnidadeConsumidoraFactory : Factory<UnidadeConsumidora, UnidadeConsumidoraFactory>
    {
        protected override UnidadeConsumidora Definition()
        {
            var instituicao = InstituicaoFactory.New().Make();
            var tipoDespesa = TipoDespesaFactory.New().Make();

            return new UnidadeConsumidora
            {
                Identificador = $"UC-{NextSequence():D6}",
                Instituicao = instituicao,
                Secretaria = instituicao.Secretaria,
                TipoDespesa = tipoDespesa,
                Orcamento = OrcamentoFactory.New().ParaInstituicao(instituicao).ParaTipoDespesa(tipoDespesa).Make(),
                Fornecedor = FornecedorFactory.New().ParaTipoDespesa(tipoDespesa).Make(),
                Excluido = false
            };
        }

        /// <summary>
        /// Vincula a um orçamento existente, herdando dele instituição, secretaria e tipo de despesa.
        /// O orçamento deve ter <c>Instituicao</c> e <c>TipoDespesa</c> carregados.
        /// </summary>
        public UnidadeConsumidoraFactory ParaOrcamento(Orcamento orcamento) => State(uc =>
        {
            var instituicao = orcamento.Instituicao
                ?? throw new InvalidOperationException("Orcamento.Instituicao precisa estar carregado.");

            uc.Orcamento = orcamento;
            uc.IdOrcamento = orcamento.IdOrcamento;
            uc.Instituicao = instituicao;
            uc.IdInstituicao = instituicao.Id;
            uc.Secretaria = instituicao.Secretaria;
            uc.IdSecretaria = instituicao.IdSecretaria;
            uc.TipoDespesa = orcamento.TipoDespesa;
            uc.IdTipoDespesa = orcamento.IdTipoDespesa;
        });

        public UnidadeConsumidoraFactory ParaFornecedor(Fornecedor fornecedor) => State(uc =>
        {
            uc.Fornecedor = fornecedor;
            uc.IdFornecedor = fornecedor.IdFornecedor;
        });

        public UnidadeConsumidoraFactory ComIdentificador(string identificador) => State(uc => uc.Identificador = identificador);

        public UnidadeConsumidoraFactory Excluida() => State(uc =>
        {
            uc.Excluido = true;
            uc.DataExclusao = DateTime.UtcNow;
        });
    }
}
