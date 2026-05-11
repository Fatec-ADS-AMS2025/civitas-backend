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
                .AnyAsync(d => d.UnidadeConsumidora.IdOrcamento == idOrcamento);
        }

        public async Task<decimal> SumValorPrevistoByOrcamentoAsync(int idOrcamento)
        {
            var total = await _context.Despesas
                .AsNoTracking()
                .Where(despesa => despesa.UnidadeConsumidora.IdOrcamento == idOrcamento)
                .SumAsync(despesa => (decimal?)despesa.ValorPrevisto) ?? 0m;

            return Math.Round(total, 2, MidpointRounding.AwayFromZero);
        }

        public async Task CalcularMediaOrcamentoEPreencherPorMes(Orcamento orcamento)
        {
            decimal valorMensal = Math.Floor((orcamento.ValorOrcamento / 12) * 100) / 100;
            decimal resto = orcamento.ValorOrcamento - (valorMensal * 12);

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
        }
    }
}
