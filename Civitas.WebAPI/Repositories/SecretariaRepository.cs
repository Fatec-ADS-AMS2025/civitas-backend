using Microsoft.EntityFrameworkCore;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Models;
using Civitas.WebAPI.Repositories.Interfaces;
using Civitas.WebAPI.Generic.Repositories;

namespace Civitas.WebAPI.Repositories;

/// <summary>
/// Repositório específico para Secretaria
/// Herda operações CRUD básicas do repositório genérico
/// </summary>
public class SecretariaRepository : GenericRepository<Secretaria>, ISecretariaRepository
{
    public SecretariaRepository(CivitasDbContext context) : base(context)
    {
    }

    public async Task<Secretaria?> GetByCnpjAsync(string cnpj)
    {
        return await GetFirstOrDefaultAsync(s => s.Cnpj == cnpj);
    }

    public async Task<Secretaria?> GetByEmailAsync(string email)
    {
        return await GetFirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<bool> CnpjExistsAsync(string cnpj, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await ExistsAsync(s => s.Cnpj == cnpj && s.IdSecretaria != excludeId.Value);
        }
        
        return await ExistsAsync(s => s.Cnpj == cnpj);
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        if (excludeId.HasValue)
        {
            return await ExistsAsync(s => s.Email == email && s.IdSecretaria != excludeId.Value);
        }
        
        return await ExistsAsync(s => s.Email == email);
    }
    
    // Override para ordenar por nome ao buscar todos
    public override async Task<IEnumerable<Secretaria>> GetAllAsync()
    {
        return await _context.Secretarias
            .OrderBy(s => s.Nome)
            .ToListAsync();
    }
}