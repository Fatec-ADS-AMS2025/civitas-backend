using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class DespesaRepository : GenericRepository<Despesa>, IDespesaRepository
    {
        private readonly AppDbContext _context;

        public DespesaRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Despesa>> GetByNumeroDocumentoAsync(string numeroDocumento)
        {
            var normalizedNumeroDocumento = numeroDocumento.Trim().ToUpperInvariant();

            return await _context.Despesas
                .AsNoTracking()
                .Where(despesa =>
                    despesa.NumeroDocumento != null &&
                    despesa.NumeroDocumento.Trim().ToUpper() == normalizedNumeroDocumento)
                .ToListAsync();
        }
        public async Task<IEnumerable<Despesa>> GetByNomeDocumentoAsync(string nomeDocumento)
        {
            var normalized = nomeDocumento.Trim().ToUpperInvariant();

            return await _context.Despesas
                .AsNoTracking()
                .Where(d =>
                    d.NomeDocumento != null &&
                    d.NomeDocumento.Trim().ToUpper() == normalized)
                .ToListAsync();
        }

        public async Task<IEnumerable<Despesa>> GetByCodigoAsync(string codigo)
        {
            var normalizedCodigo = codigo.Trim().ToUpperInvariant();

            return await _context.Despesas
                .AsNoTracking()
                .Where(despesa =>
                    despesa.Codigo != null &&
                    despesa.Codigo.Trim().ToUpper() == normalizedCodigo)
                .ToListAsync();
        }

        public async Task<IEnumerable<Despesa>> GetByUnidadeConsumidoraAsync(int idUnidadeConsumidora)
        {
            return await _context.Despesas
                .AsNoTracking()
                .Where(despesa => despesa.IdUnidadeConsumidora == idUnidadeConsumidora)
                .ToListAsync();
        }

        public async Task<IEnumerable<Despesa>> GetByUsuarioAsync(int idUsuario)
        {
            return await _context.Despesas
                .AsNoTracking()
                .Where(despesa => despesa.IdUsuario == idUsuario)
                .ToListAsync();
        }

        public async Task<IEnumerable<Despesa>> GetByStatusAsync(Status status)
        {
            return await _context.Despesas
                .AsNoTracking()
                .Where(despesa => despesa.Status == status)
                .ToListAsync();
        }

        public async Task<decimal> SumValorPrevistoByOrcamentoAsync(int idOrcamento, int? ignoreId = null)
        {
            var query = _context.Despesas
                .AsNoTracking()
                .Where(despesa => despesa.UnidadeConsumidora.IdOrcamento == idOrcamento);

            if (ignoreId.HasValue)
            {
                query = query.Where(despesa => despesa.Id != ignoreId.Value);
            }

            var total = await query.SumAsync(despesa => (decimal?)despesa.ValorPrevisto) ?? 0m;
            return Math.Round(total, 2, MidpointRounding.AwayFromZero);
        }

        public async Task<bool> ExistsByHashDocumentoAsync(string hashDocumento, int? ignoreId = null)
        {
            var query = _context.Despesas
                .AsNoTracking()
                .Where(despesa => despesa.HashDocumento == hashDocumento);

            if (ignoreId.HasValue)
            {
                query = query.Where(despesa => despesa.Id != ignoreId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
