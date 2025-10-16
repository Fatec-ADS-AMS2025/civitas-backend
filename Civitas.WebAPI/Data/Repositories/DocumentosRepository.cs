using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class DocumentosRepository : GenericRepository<Documentos>, IDocumentosRepository
    {
        private readonly AppDbContext _context;

        public DocumentosRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
