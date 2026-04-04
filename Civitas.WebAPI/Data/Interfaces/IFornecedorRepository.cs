using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IFornecedorRepository : IGenericRepository<Fornecedor>
    {
        Task<bool> ExistsByCnpjAsync(string cnpj, int? ignoreId = null);
    }
}
