using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Objects.Dtos.Entities;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface ISecretariaRepository : IGenericRepository<Secretaria>
    {
    Task<bool> ExistsByCnpjAsync(string cnpj, int? ignoreId = null);
    Task<bool> ExistsByEmailAsync(string email, int? ignoreId = null);
    Task<SecretariaGastosDTO?> GetGastosBySecretariaIdAsync(int secretariaId);
    Task<SecretariaOrcamentoDisponivelDTO?> GetOrcamentoDisponivelBySecretariaIdAsync(int secretariaId);
    }
}
