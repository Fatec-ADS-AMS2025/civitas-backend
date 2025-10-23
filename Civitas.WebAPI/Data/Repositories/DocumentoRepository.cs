using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class DocumentoRepository : GenericRepository<Documento>, IDocumentoRepository
    {
        private readonly AppDbContext _context;

        public DocumentoRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
