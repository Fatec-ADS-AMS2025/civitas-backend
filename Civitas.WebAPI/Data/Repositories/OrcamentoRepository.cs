using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class OrcamentoRepository : GenericRepository<Orcamento>, IOrcamentoRepository
    {
        private readonly AppDbContext _context;

        public OrcamentoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExisteDespesaVinculada(int idOrcamento)
        {
            return await _context.Despesas
                .AnyAsync(d => d.UnidadeConsumidora.IdOrcamento == idOrcamento && !d.Excluido);
        }

        public async Task<decimal> SumValorPrevistoByOrcamentoAsync(int idOrcamento)
        {
            var total = await _context.Despesas
                .AsNoTracking()
                .Where(despesa => despesa.UnidadeConsumidora.IdOrcamento == idOrcamento && !despesa.Excluido)
                .SumAsync(despesa => (decimal?)despesa.ValorPrevisto) ?? 0m;

            return Math.Round(total, 2, MidpointRounding.AwayFromZero);
        }

        public async Task CalcularMediaOrcamentoEPreencherPorMes(Orcamento orcamento)
        {
            if (orcamento.ValorOrcamento != 0 && orcamento.ValorOrcamento != null)
            {
                decimal valorMensal = Math.Floor(((orcamento.ValorOrcamento ?? 0) / 12) * 100) / 100;
                decimal resto = (orcamento.ValorOrcamento ?? 0) - (valorMensal * 12);

                orcamento.JaneiroValorConsumo = valorMensal;
                orcamento.FevereiroValorConsumo = valorMensal;
                orcamento.MarcoValorConsumo = valorMensal;
                orcamento.AbrilValorConsumo = valorMensal;
                orcamento.MaioValorConsumo = valorMensal;
                orcamento.JunhoValorConsumo = valorMensal;
                orcamento.JulhoValorConsumo = valorMensal;
                orcamento.AgostoValorConsumo = valorMensal;
                orcamento.SetembroValorConsumo = valorMensal;
                orcamento.OutubroValorConsumo = valorMensal;
                orcamento.NovembroValorConsumo = valorMensal;
                orcamento.DezembroValorConsumo = valorMensal + resto;
            } else
            {
                decimal totalOrcamento = (orcamento.JaneiroValorConsumo ?? 0) + (orcamento.FevereiroValorConsumo ?? 0) + (orcamento.MarcoValorConsumo ?? 0) + (orcamento.AbrilValorConsumo ?? 0) +
                    (orcamento.MaioValorConsumo ?? 0) + (orcamento.JunhoValorConsumo ?? 0) + (orcamento.JulhoValorConsumo ?? 0) +
                    (orcamento.AgostoValorConsumo ?? 0) + (orcamento.SetembroValorConsumo ?? 0) + (orcamento.OutubroValorConsumo ?? 0) +
                    (orcamento.NovembroValorConsumo ?? 0) + (orcamento.DezembroValorConsumo ?? 0);

                orcamento.ValorOrcamento = totalOrcamento;
            }
            
        }
    }
}
