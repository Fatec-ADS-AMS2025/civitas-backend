using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<bool> ExistsByCpf(string cpf, int? ignoredId = null);
        Task<bool> ExistsByEmail(string email, int? ignoredId = null);
        Task<bool> ExistsByMatricula(string matricula, int? ignoredId = null);
        Task<IEnumerable<Usuario>> GetUsuarioByCpf(string cpf);
    }
}
