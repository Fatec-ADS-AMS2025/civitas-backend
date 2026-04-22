using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
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
