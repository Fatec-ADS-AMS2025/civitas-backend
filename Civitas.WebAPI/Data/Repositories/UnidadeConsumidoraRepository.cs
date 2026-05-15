using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Contracts;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class UnidadeConsumidoraRepository : GenericRepository<UnidadeConsumidora>, IUnidadeConsumidoraRepository
    {
        private readonly AppDbContext _context;

        public UnidadeConsumidoraRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<UnidadeConsumidora>> GetPageNaoExcluidos(PaginationQuery paginationQuery)
        {
            return await PageByExcluidoAsync(paginationQuery, excluido: false);
        }

        public async Task<PaginatedResult<UnidadeConsumidora>> GetPageExcluidos(PaginationQuery paginationQuery)
        {
            return await PageByExcluidoAsync(paginationQuery, excluido: true);
        }

        public async Task<UnidadeConsumidora?> GetByIdNaoExcluidoAsync(int id)
        {
            return await _context.UnidadesConsumidoras
                .AsNoTracking()
                .FirstOrDefaultAsync(uc => uc.Id == id && !uc.Excluido);
        }

        public async Task<UnidadeConsumidora?> GetByIdentificadorNaoExcluidoAsync(string identificador)
        {
            return await _context.UnidadesConsumidoras
                .AsNoTracking()
                .FirstOrDefaultAsync(uc => uc.Identificador == identificador && !uc.Excluido);
        }

        public async Task<IEnumerable<UnidadeConsumidora>> GetByInstituicaoNaoExcluidosAsync(int idInstituicao)
        {
            return await _context.UnidadesConsumidoras
                .AsNoTracking()
                .Where(uc => uc.IdInstituicao == idInstituicao && !uc.Excluido)
                .ToListAsync();
        }

        public async Task<IEnumerable<UnidadeConsumidora>> GetBySecretariaNaoExcluidosAsync(int idSecretaria)
        {
            return await _context.UnidadesConsumidoras
                .AsNoTracking()
                .Where(uc => uc.IdSecretaria == idSecretaria && !uc.Excluido)
                .ToListAsync();
        }

        public async Task<bool> ExistsByIdentificadorAsync(string identificador, int? ignoreId = null)
        {
            var query = _context.UnidadesConsumidoras
                .AsNoTracking()
                .Where(uc => uc.Identificador == identificador && !uc.Excluido);

            if (ignoreId.HasValue)
            {
                query = query.Where(uc => uc.Id != ignoreId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> InstituicaoExistsAsync(int idInstituicao)
        {
            return await _context.Instituicoes.AsNoTracking().AnyAsync(i => i.Id == idInstituicao && !i.Excluido);
        }

        public async Task<bool> TipoDespesaExistsAsync(int idTipoDespesa)
        {
            return await _context.TiposDespesa.AsNoTracking().AnyAsync(td => td.Id == idTipoDespesa && !td.Excluido);
        }

        public async Task<bool> SecretariaExistsAsync(int idSecretaria)
        {
            return await _context.Secretarias.AsNoTracking().AnyAsync(s => s.IdSecretaria == idSecretaria && !s.Excluido);
        }

        public async Task<bool> OrcamentoExistsAsync(int idOrcamento)
        {
            return await _context.Orcamentos.AsNoTracking().AnyAsync(o => o.IdOrcamento == idOrcamento && !o.Excluido);
        }

        public async Task<bool> FornecedorExistsAsync(int idFornecedor)
        {
            return await _context.Fornecedores.AsNoTracking().AnyAsync(f => f.IdFornecedor == idFornecedor && !f.Excluido);
        }

        private async Task<PaginatedResult<UnidadeConsumidora>> PageByExcluidoAsync(PaginationQuery paginationQuery, bool excluido)
        {
            var currentPage = paginationQuery.NormalizedPage;
            var pageSize = paginationQuery.NormalizedSize;

            var query = _context.UnidadesConsumidoras
                .AsNoTracking()
                .Where(uc => uc.Excluido == excluido)
                .OrderBy(uc => uc.Id);

            var totalRecords = await query.CountAsync();
            var items = await query
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResult<UnidadeConsumidora>
            {
                Items = items,
                TotalRecords = totalRecords,
                TotalPages = totalRecords == 0 ? 0 : (int)Math.Ceiling(totalRecords / (double)pageSize),
                CurrentPage = currentPage,
                PageSize = pageSize
            };
        }
    }
}
