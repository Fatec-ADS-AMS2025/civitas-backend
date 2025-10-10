using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.AspNetCore.Mvc;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IUsuarioRepository : IGenericRepository<Usuario>
    {
        Task<IEnumerable<Usuario>> GetUsuarioByCpf(string cpf);

    }
}
