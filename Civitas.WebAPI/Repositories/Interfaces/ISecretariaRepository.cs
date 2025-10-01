using Civitas.WebAPI.Models;

namespace Civitas.WebAPI.Repositories.Interfaces;

public interface ISecretariaRepository
{
    Task<IEnumerable<Secretaria>> GetAllAsync();
    Task<IEnumerable<Secretaria>> GetActiveAsync();
    Task<Secretaria?> GetByIdAsync(int id);
    Task<Secretaria?> GetByCnpjAsync(string cnpj);
    Task<Secretaria?> GetByEmailAsync(string email);
    Task<Secretaria> CreateAsync(Secretaria secretaria);
    Task<Secretaria> UpdateAsync(Secretaria secretaria);
    Task<bool> DeleteAsync(int id);
    Task<bool> ActivateAsync(int id);
    Task<bool> DeactivateAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> CnpjExistsAsync(string cnpj, int? excludeId = null);
    Task<bool> EmailExistsAsync(string email, int? excludeId = null);
}