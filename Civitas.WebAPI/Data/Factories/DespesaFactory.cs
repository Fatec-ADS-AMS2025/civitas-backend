using System.Security.Cryptography;
using System.Text;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>
    /// Factory de <see cref="Despesa"/> (padrão: a pagar, vencendo nos próximos 30 dias).
    /// Cria <see cref="Usuario"/> e <see cref="UnidadeConsumidora"/> quando não informados.
    /// </summary>
    public class DespesaFactory : Factory<Despesa, DespesaFactory>
    {
        protected override Despesa Definition()
        {
            var sequence = NextSequence();
            var hoje = DateOnly.FromDateTime(DateTime.UtcNow);
            var emissao = hoje.AddDays(-Faker.Random.Int(5, 20));
            var consumo = Math.Round(Faker.Random.Decimal(10m, 5_000m), 2);
            var numeroDocumento = $"NF-{hoje.Year}-{sequence:D5}";

            return new Despesa
            {
                NumeroDocumento = numeroDocumento,
                NomeDocumento = $"Documento {sequence}",
                HashDocumento = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(numeroDocumento))),
                Codigo = Faker.Random.Int(1000, 9999).ToString(),
                DataEmissao = emissao,
                DataVencimento = hoje.AddDays(Faker.Random.Int(1, 30)),
                DataPagamento = null,
                ValorPrevisto = Math.Round(Faker.Random.Decimal(100m, 50_000m), 2),
                ValorPago = 0m,
                Juros = 0m,
                Multa = 0m,
                Desconto = 0m,
                ConsumoPrevisto = consumo,
                ConsumoReal = 0m,
                Status = Status.A_PAGAR,
                Usuario = UsuarioFactory.New().Make(),
                UnidadeConsumidora = UnidadeConsumidoraFactory.New().Make()
            };
        }

        public DespesaFactory ParaUsuario(Usuario usuario) => State(d =>
        {
            d.Usuario = usuario;
            d.IdUsuario = usuario.Id;
        });

        public DespesaFactory ParaUnidadeConsumidora(UnidadeConsumidora unidadeConsumidora) => State(d =>
        {
            d.UnidadeConsumidora = unidadeConsumidora;
            d.IdUnidadeConsumidora = unidadeConsumidora.Id;
        });

        public DespesaFactory APagar() => State(d =>
        {
            d.Status = Status.A_PAGAR;
            d.ValorPago = 0m;
            d.ConsumoReal = 0m;
            d.DataPagamento = null;
        });

        public DespesaFactory Paga() => State(d =>
        {
            d.Status = Status.PAGA;
            d.ValorPago = d.ValorPrevisto;
            d.ConsumoReal = d.ConsumoPrevisto;
            d.DataEmissao = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-Faker.Random.Int(20, 90));
            d.DataVencimento = d.DataEmissao.AddDays(15);
            d.DataPagamento = d.DataVencimento.AddDays(-1);
        });

        public DespesaFactory Atrasada() => State(d =>
        {
            d.Status = Status.ATRASADO;
            d.ValorPago = 0m;
            d.DataPagamento = null;
            d.DataVencimento = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(-Faker.Random.Int(1, 30));
            d.DataEmissao = d.DataVencimento.AddDays(-15);
        });

        /// <summary>Preenche os valores dos campos opcionais (<c>{"campo": "valor"}</c>).</summary>
        public DespesaFactory ComValoresOpcionais(IDictionary<string, string?> valores) => State(d =>
            d.ValoresOpcionais = System.Text.Json.JsonSerializer.Serialize(valores));
    }
}
