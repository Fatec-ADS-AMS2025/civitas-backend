using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class EmailRepository : GenericRepository<EmailEnviado>, IEmailRepository
    {
        private readonly AppDbContext _context;

        public EmailRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<IEnumerable<EmailEnviado>> Get()
        {
            return await _context.EmailsEnviados
                .AsNoTracking()
                .OrderByDescending(email => email.DataCriacao)
                .ThenByDescending(email => email.Id)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<EmailEnviado>> GetByDestinatarioAsync(string emailDestinatario)
        {
            return await _context.EmailsEnviados
                .AsNoTracking()
                .Where(email => email.EmailDestinatario == emailDestinatario)
                .OrderByDescending(email => email.DataCriacao)
                .ThenByDescending(email => email.Id)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<EmailEnviado>> GetByStatusAsync(EmailStatusEnum status)
        {
            return await _context.EmailsEnviados
                .AsNoTracking()
                .Where(email => email.Status == status)
                .OrderByDescending(email => email.DataCriacao)
                .ThenByDescending(email => email.Id)
                .ToListAsync();
        }
    }
}
