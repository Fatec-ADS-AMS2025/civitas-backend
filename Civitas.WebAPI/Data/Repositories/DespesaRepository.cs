using Civitas.WebAPI.Data.Interfaces;
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

        public async Task<bool> ExistsByNumeroDocumentoAndFornecedorAsync(
            string numeroDocumento,
            int idFornecedor,
            int? ignoreId = null)
        {
            var normalizedNumeroDocumento = numeroDocumento.Trim().ToUpperInvariant();

            var query = _context.Despesas
                .AsNoTracking()
                .Where(despesa =>
                    despesa.IdFornecedor == idFornecedor &&
                    despesa.NumeroDocumento != null &&
                    despesa.NumeroDocumento.Trim().ToUpper() == normalizedNumeroDocumento);

            if (ignoreId.HasValue)
            {
                query = query.Where(despesa => despesa.Id != ignoreId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
