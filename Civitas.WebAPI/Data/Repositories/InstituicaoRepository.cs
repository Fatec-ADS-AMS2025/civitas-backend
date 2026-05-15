using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class InstituicaoRepository : GenericRepository<Instituicao>, IInstituicaoRepository
    {
        private readonly AppDbContext _context;

        public InstituicaoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Instituicao>> GetInstituicaoByName(string name)
        {
            return await _context.Instituicoes.Where(m => m.Nome.Contains(name) && !m.Excluido).ToListAsync();
        }

        public async Task<bool> ExistsByCnpjAsync(string cnpj, int? ignoreId = null)
        {
            var query = _context.Instituicoes
                .AsNoTracking()
                .Where(instituicao => instituicao.CNPJ == cnpj && !instituicao.Excluido);

            if (ignoreId.HasValue)
            {
                query = query.Where(instituicao => instituicao.Id != ignoreId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> ExistsByEmailAsync(string email, int? ignoreId = null)
        {
            var normalizedEmail = email.ToLowerInvariant();

            var query = _context.Instituicoes
                .AsNoTracking()
                .Where(instituicao => instituicao.Email.ToLower() == normalizedEmail && !instituicao.Excluido);

            if (ignoreId.HasValue)
            {
                query = query.Where(instituicao => instituicao.Id != ignoreId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> HasDespesasPendentesAsync(int instituicaoId)
        {
            return await _context.Despesas
                .AsNoTracking()
                .AnyAsync(despesa =>
                    despesa.UnidadeConsumidora.IdInstituicao == instituicaoId &&
                    despesa.Status == Status.A_PAGAR &&
                    !despesa.Excluido);
        }

        public async Task<InstituicaoGastosDTO?> GetGastosByInstituicaoIdAsync(int instituicaoId, int tipoDespesaId)
        {
            var instituicao = await _context.Instituicoes
                .AsNoTracking()
                .FirstOrDefaultAsync(i => i.Id == instituicaoId && !i.Excluido);

            if (instituicao is null)
                return null;

            var despesas = await _context.Despesas
                .AsNoTracking()
                .Where(d =>
                    d.UnidadeConsumidora.IdInstituicao == instituicaoId &&
                    d.UnidadeConsumidora.IdTipoDespesa == tipoDespesaId &&
                    !d.Excluido
                )
                .ToListAsync();

            return new InstituicaoGastosDTO
            {
                IdInstituicao = instituicao.Id,
                NomeInstituicao = instituicao.Nome,

                QuantidadeDespesas = despesas.Count,
                TotalGastos = despesas.Sum(d => d.ValorPago),

                JaneiroGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 1)
                    .Sum(d => d.ValorPago),

                FevereiroGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 2)
                    .Sum(d => d.ValorPago),

                MarcoGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 3)
                    .Sum(d => d.ValorPago),

                AbrilGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 4)
                    .Sum(d => d.ValorPago),

                MaioGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 5)
                    .Sum(d => d.ValorPago),

                JunhoGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 6)
                    .Sum(d => d.ValorPago),

                JulhoGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 7)
                    .Sum(d => d.ValorPago),

                AgostoGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 8)
                    .Sum(d => d.ValorPago),

                SetembroGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 9)
                    .Sum(d => d.ValorPago),

                OutubroGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 10)
                    .Sum(d => d.ValorPago),

                NovembroGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 11)
                    .Sum(d => d.ValorPago),

                DezembroGastos = despesas
                    .Where(d => d.DataPagamento.HasValue && d.DataPagamento.Value.Month == 12)
                    .Sum(d => d.ValorPago)
            };
        }

        public async Task<InstituicaoOrcamentoDisponivelDTO?> GetOrcamentoDisponivelByInstituicaoIdAsync(int instituicaoId)
        {
            return await _context.Instituicoes
                .AsNoTracking()
                .Where(instituicao => instituicao.Id == instituicaoId && !instituicao.Excluido)
                .Select(instituicao => new InstituicaoOrcamentoDisponivelDTO
                {
                    IdInstituicao = instituicao.Id,
                    NomeInstituicao = instituicao.Nome,
                    TotalOrcamentoDisponivel =
                        (instituicao.Orcamento.Where(orcamento => !orcamento.Excluido).Sum(orcamento => (decimal?)orcamento.ValorOrcamento) ?? 0m)
                        - (_context.Despesas
                            .Where(despesa => despesa.UnidadeConsumidora.IdInstituicao == instituicao.Id && !despesa.Excluido)
                            .Sum(despesa => (decimal?)despesa.ValorPrevisto) ?? 0m)
                })
                .FirstOrDefaultAsync();
        }
    }
}
