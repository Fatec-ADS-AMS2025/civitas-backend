using Civitas.WebAPI.Objects.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de Transferência de Dados (DTO) utilizado para trafegar informações completas do Usuário.
    /// </summary>
    /// <remarks>
    /// Utilização:
    /// - Entrada (Input): Cadastro (POST) e Atualização (PUT) de usuários.
    /// - Saída (Output): Detalhes de perfil do usuário.
    /// </remarks>
    public class UsuarioDTO
    {
        /// <summary>
        /// Identificador único do usuário.
        /// </summary>
        /// <remarks>
        /// Entrada: Deve ser ignorado ou nulo na criação (POST). Obrigatório na atualização (PUT).
        /// Saída: Retorna o ID gerado pelo banco.
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// CPF do usuário (Cadastro de Pessoa Física).
        /// </summary>
        /// <remarks>
        /// Obrigatório. O sistema deve validar o formato e unicidade deste campo.
        /// </remarks>
        public string Cpf { get; set; }

        /// <summary>
        /// Nome completo do usuário.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Usado para exibição em relatórios e telas de perfil.
        /// </remarks>
        public string Nome { get; set; }

        /// <summary>
        /// Registro Geral (RG).
        /// </summary>
        public string Rg { get; set; }

        /// <summary>
        /// Logradouro do endereço (Rua, Av.).
        /// </summary>
        public string Logradouro { get; set; }

        /// <summary>
        /// Número do endereço.
        /// </summary>
        public string Numero { get; set; }

        /// <summary>
        /// Cidade de residência.
        /// </summary>
        public string Cidade { get; set; }

        /// <summary>
        /// Estado (UF).
        /// </summary>
        /// <example>SP, RJ, MG.</example>
        public string Estado { get; set; }

        /// <summary>
        /// Código Postal (CEP).
        /// </summary>
        public string Cep { get; set; }

        /// <summary>
        /// Bairro do endereço.
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// E-mail de contato e login.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Deve ser um formato de e-mail válido.
        /// </remarks>
        public string Email { get; set; }

        /// <summary>
        /// Senha de acesso ao sistema.
        /// </summary>
        /// <remarks>
        /// Entrada: Deve ser enviada em texto plano ou hash pelo front-end para validação/criação.
        /// Saída: Por questões de segurança, recomenda-se retornar nulo ou o hash mascarado em operações de GET.
        /// </remarks>
        public string Senha { get; set; }

        /// <summary>
        /// Matrícula interna do funcionário.
        /// </summary>
        /// <remarks>
        /// Relevante apenas se o TipoUsuario for FUNCIONARIO ou ADMINISTRADOR.
        /// </remarks>
        public string Matricula { get; set; }

        /// <summary>
        /// Estado da conta do usuário.
        /// </summary>
        /// <remarks>
        /// Valores: <see cref="Civitas.WebAPI.Objects.Enums.Situacao"/> (1-Ativo, 2-Inativo).
        /// </remarks>
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Perfil de permissão do usuário.
        /// </summary>
        /// <remarks>
        /// Valores: <see cref="Civitas.WebAPI.Objects.Enums.TipoUsuario"/> (Visitante, Admin, Funcionário).
        /// </remarks>
        public TipoUsuario TipoUsuario { get; set; }
    }
}