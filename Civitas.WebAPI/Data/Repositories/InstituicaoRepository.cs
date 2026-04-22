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
            return await _context.Instituicoes.Where(m => m.Nome.Contains(name)).ToListAsync();
        }

        public async Task<bool> ExistsByCnpjAsync(string cnpj, int? ignoreId = null)
        {
            var query = _context.Instituicoes
                .AsNoTracking()
                .Where(instituicao => instituicao.CNPJ == cnpj);

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
                .Where(instituicao => instituicao.Email.ToLower() == normalizedEmail);

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
                .AnyAsync(despesa => despesa.IdInstituicao == instituicaoId && despesa.Status == Status.A_PAGAR);
        }

        public async Task<InstituicaoGastosDTO?> GetGastosByInstituicaoIdAsync(int instituicaoId)
        {
            return await _context.Instituicoes
                .AsNoTracking()
                .Where(instituicao => instituicao.Id == instituicaoId)
                .Select(instituicao => new InstituicaoGastosDTO
                {
                    IdInstituicao = instituicao.Id,
                    NomeInstituicao = instituicao.Nome,
                    QuantidadeDespesas = instituicao.Despesas.Count(),
                    TotalGastos = instituicao.Despesas
                        .Sum(despesa => despesa.ConsumoPrevisto)
                })
                .FirstOrDefaultAsync();
        }

        public async Task<InstituicaoOrcamentoDisponivelDTO?> GetOrcamentoDisponivelByInstituicaoIdAsync(int instituicaoId)
        {
            return await _context.Instituicoes
                .AsNoTracking()
                .Where(instituicao => instituicao.Id == instituicaoId)
                .Select(instituicao => new InstituicaoOrcamentoDisponivelDTO
                {
                    IdInstituicao = instituicao.Id,
                    NomeInstituicao = instituicao.Nome,
                    TotalOrcamentoDisponivel =
                        (instituicao.Orcamento.Sum(orcamento => (decimal?)orcamento.ValorOrcamento) ?? 0m)
                        - (instituicao.Despesas.Sum(despesa => (decimal?)despesa.ConsumoPrevisto) ?? 0m)
                })
                .FirstOrDefaultAsync();
        }
    }
}
