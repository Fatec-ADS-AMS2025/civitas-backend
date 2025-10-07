using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    public interface IUsuarioService : IGenericService<Usuario, UsuarioDTO>
    {
        Task<IEnumerable<UsuarioDTO>> GetUsuarioByCpf(string cpf);
        Task UpdateSituacao(int id, Situacao situacao);
    }
}
