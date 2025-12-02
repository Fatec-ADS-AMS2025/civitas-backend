using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Interface que define o contrato para o serviço de gestão de Usuários, Acesso e Segurança.
    /// </summary>
    /// <remarks>
    /// Herda as operações padrão de <see cref="IGenericService{Usuario, UsuarioDTO}"/>.
    /// É o componente central para injeção de dependência em rotinas de autenticação, verificação de duplicidade e gestão de perfis.
    /// </remarks>
    public interface IUsuarioService : IGenericService<Usuario, UsuarioDTO>
    {
        /// <summary>
        /// Realiza a busca de usuários através do número de CPF (Cadastro de Pessoa Física).
        /// </summary>
        /// <param name="cpf">O número do CPF a ser pesquisado.</param>
        /// <returns>Uma coleção contendo o(s) usuário(s) encontrado(s).</returns>
        /// <remarks>
        /// Contrato de Identidade:
        /// Método essencial para validar a regra de negócio que impede o cadastro duplicado de pessoas físicas.
        /// Também utilizado como chave de busca em processos de recuperação de senha.
        /// </remarks>
        Task<IEnumerable<UsuarioDTO>> GetUsuarioByCpf(string cpf);
    }
}