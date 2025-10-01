using Microsoft.EntityFrameworkCore;
using Civitas.WebAPI.Data;
using Civitas.WebAPI.Models;
using Civitas.WebAPI.Repositories.Interfaces;

namespace Civitas.WebAPI.Repositories;

public class SecretariaRepository : ISecretariaRepository
{
    private readonly CivitasDbContext _context;

    public SecretariaRepository(CivitasDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Secretaria>> GetAllAsync()
    {
        return await _context.Secretarias
            .OrderBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<IEnumerable<Secretaria>> GetActiveAsync()
    {
        return await _context.Secretarias
            .Where(s => s.Ativo)
            .OrderBy(s => s.Nome)
            .ToListAsync();
    }

    public async Task<Secretaria?> GetByIdAsync(int id)
    {
        return await _context.Secretarias
            .FirstOrDefaultAsync(s => s.IdSecretaria == id);
    }

    public async Task<Secretaria?> GetByCnpjAsync(string cnpj)
    {
        return await _context.Secretarias
            .FirstOrDefaultAsync(s => s.Cnpj == cnpj);
    }

    public async Task<Secretaria?> GetByEmailAsync(string email)
    {
        return await _context.Secretarias
            .FirstOrDefaultAsync(s => s.Email == email);
    }

    public async Task<Secretaria> CreateAsync(Secretaria secretaria)
    {
        secretaria.DataCriacao = DateTime.UtcNow;
        secretaria.Ativo = true;
        
        _context.Secretarias.Add(secretaria);
        await _context.SaveChangesAsync();
        return secretaria;
    }

    public async Task<Secretaria> UpdateAsync(Secretaria secretaria)
    {
        secretaria.DataAlteracao = DateTime.UtcNow;
        
        _context.Secretarias.Update(secretaria);
        await _context.SaveChangesAsync();
        return secretaria;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var secretaria = await GetByIdAsync(id);
        if (secretaria == null)
            return false;

        _context.Secretarias.Remove(secretaria);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActivateAsync(int id)
    {
        var secretaria = await GetByIdAsync(id);
        if (secretaria == null)
            return false;

        secretaria.Ativo = true;
        secretaria.DataAlteracao = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeactivateAsync(int id)
    {
        var secretaria = await GetByIdAsync(id);
        if (secretaria == null)
            return false;

        secretaria.Ativo = false;
        secretaria.DataAlteracao = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Secretarias
            .AnyAsync(s => s.IdSecretaria == id);
    }

    public async Task<bool> CnpjExistsAsync(string cnpj, int? excludeId = null)
    {
        var query = _context.Secretarias.Where(s => s.Cnpj == cnpj);
        
        if (excludeId.HasValue)
            query = query.Where(s => s.IdSecretaria != excludeId.Value);
            
        return await query.AnyAsync();
    }

    public async Task<bool> EmailExistsAsync(string email, int? excludeId = null)
    {
        var query = _context.Secretarias.Where(s => s.Email == email);
        
        if (excludeId.HasValue)
            query = query.Where(s => s.IdSecretaria != excludeId.Value);
            
        return await query.AnyAsync();
    }
}