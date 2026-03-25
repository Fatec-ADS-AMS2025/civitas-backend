using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<bool> ExistsByCpf(string cpf, int? ignoredId = null);
        Task<bool> ExistsByEmail(string email, int? ignoredId = null);
        Task<bool> ExistsByMatricula(string matricula, int? ignoredId = null);
        Task<Usuario?> GetByEmailAsync(string email);
        Task<IEnumerable<Usuario>> GetUsuarioByCpf(string cpf);
    }
}
