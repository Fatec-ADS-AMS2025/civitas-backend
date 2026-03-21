using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface ISecretariaRepository : IGenericRepository<Secretaria>
    {
    Task<bool> ExistsByCnpjAsync(string cnpj, int? ignoreId = null);
    Task<bool> ExistsByEmailAsync(string email, int? ignoreId = null);
    }
}
