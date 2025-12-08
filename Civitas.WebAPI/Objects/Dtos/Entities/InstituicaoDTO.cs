using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência para dados de Instituições (Unidades Administrativas).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Input: Formulário de cadastro de novas escolas, postos de saúde, etc.
    /// - Output: Listagem de instituições para relatórios e gestão.
    /// </remarks>
    public class InstituicaoDTO
    {
        /// <summary>
        /// Identificador único da instituição.
        /// </summary>
        /// <remarks>
        /// Input: Deve ser 0 ou nulo na criação (POST). Obrigatório na atualização (PUT).
        /// </remarks>
        public int Id { get; set; }

        /// <summary>
        /// Cadastro Nacional da Pessoa Jurídica.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Deve ser validado (algoritmo de módulo 11) e verificado quanto à unicidade no banco.
        /// </remarks>
        public string CNPJ { get; set; }

        /// <summary>
        /// Nome Fantasia (Nome comum).
        /// </summary>
        /// <example>Escola Municipal Alegria, Hospital Central.</example>
        public string Nome { get; set; }

        /// <summary>
        /// Logradouro do endereço.
        /// </summary>
        public string Logradouro { get; set; }

        /// <summary>
        /// Número do imóvel.
        /// </summary>
        public string Numero { get; set; }

        /// <summary>
        /// Bairro.
        /// </summary>
        public string Bairro { get; set; }

        /// <summary>
        /// CEP do endereço.
        /// </summary>
        public string CEP { get; set; }

        /// <summary>
        /// Razão Social oficial.
        /// </summary>
        public string NomeRazaoSocial { get; set; }

        /// <summary>
        /// Telefone de contato.
        /// </summary>
        public string Telefone { get; set; }

        /// <summary>
        /// E-mail institucional.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Validar formato de e-mail.
        /// </remarks>
        public string Email { get; set; }

        /// <summary>
        /// Cidade de localização.
        /// </summary>
        public string Cidade { get; set; }

        /// <summary>
        /// Estado (UF).
        /// </summary>
        public string Estado { get; set; }

        /// <summary>
        /// Situação cadastral.
        /// </summary>
        /// <remarks>
        /// Valores: <see cref="Situacao"/>.
        /// Instituições INATIVAS não podem ter orçamentos alocados.
        /// </remarks>
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Categoria da Instituição (FK).
        /// </summary>
        /// <remarks>
        /// Obrigatório. Define se é Escola, Hospital, Departamento, etc.
        /// </remarks>
        public int IdTipoInstituicao { get; set; }

        /// <summary>
        /// Secretaria responsável (FK).
        /// </summary>
        /// <remarks>
        /// Obrigatório. Define a hierarquia superior (quem paga a conta).
        /// </remarks>
        public int IdSecretaria { get; set; }
    }
}