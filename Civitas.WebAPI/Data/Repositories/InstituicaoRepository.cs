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

        public async Task<InstituicaoGastosDTO?> GetGastosByInstituicaoIdAsync(int instituicaoId)
        {
            return await _context.Instituicoes
                .AsNoTracking()
                .Where(instituicao => instituicao.Id == instituicaoId)
                .Select(instituicao => new InstituicaoGastosDTO
                {
                    IdInstituicao = instituicao.Id,
                    NomeInstituicao = instituicao.Nome,
                    QuantidadeDespesas = instituicao.Despesas.Count(despesa => despesa.Situacao == Situacao.ATIVO),
                    TotalGastos = instituicao.Despesas
                        .Where(despesa => despesa.Situacao == Situacao.ATIVO)
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
                    TotalOrcamentoDisponivel = (instituicao.Orcamento.Sum(orcamento => orcamento.ValorOrcamento) - instituicao.Despesas
                        .Where(despesa => despesa.Situacao == Situacao.ATIVO)
                        .Sum(despesa => despesa.ConsumoPrevisto))
                })
                .FirstOrDefaultAsync();
        }
    }
}
