using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência de dados para gestão de Fornecedores (Credores).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Input: Cadastro de novas empresas prestadoras de serviço.
    /// - Output: Listagem de fornecedores para vínculo em despesas.
    /// </remarks>
    public class FornecedorDTO
    {
        /// <summary>
        /// Identificador único do fornecedor.
        /// </summary>
        /// <remarks>
        /// Input: Nulo ou zero na criação (POST). Obrigatório na edição (PUT).
        /// </remarks>
        public int IdFornecedor { get; set; }

        /// <summary>
        /// Nome Fantasia (Nome comercial).
        /// </summary>
        /// <example>Papelaria Central, Kalunga, Sabesp.</example>
        public string NomeFantasia { get; set; } = string.Empty;

        /// <summary>
        /// Situação cadastral no sistema.
        /// </summary>
        /// <remarks>
        /// Valores: <see cref="Situacao"/>.
        /// Regra: Fornecedores INATIVOS (2) não podem ser selecionados para novas Despesas.
        /// </remarks>
        public Situacao Situacao { get; set; }

        /// <summary>
        /// CNPJ (Cadastro Nacional da Pessoa Jurídica).
        /// </summary>
        /// <remarks>
        /// Obrigatório. O backend deve validar:
        /// 1. Formato (14 dígitos).
        /// 2. Dígitos verificadores (Algoritmo oficial).
        /// 3. Unicidade (não permitir duplicidade de CNPJ).
        /// </remarks>
        public string Cnpj { get; set; } = string.Empty;

        /// <summary>
        /// Razão Social oficial.
        /// </summary>
        /// <remarks>
        /// Nome jurídico utilizado para emissão de notas fiscais.
        /// </remarks>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Logradouro do endereço da sede.
        /// </summary>
        public string Logradouro { get; set; } = string.Empty;

        /// <summary>
        /// Número do imóvel.
        /// </summary>
        public string Numero { get; set; } = string.Empty;

        /// <summary>
        /// Bairro.
        /// </summary>
        public string Bairro { get; set; } = string.Empty;

        /// <summary>
        /// CEP.
        /// </summary>
        public string Cep { get; set; } = string.Empty;

        /// <summary>
        /// Telefone comercial.
        /// </summary>
        public string Telefone { get; set; } = string.Empty;

        /// <summary>
        /// E-mail para contato financeiro.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Cidade.
        /// </summary>
        public string Cidade { get; set; } = string.Empty;

        /// <summary>
        /// Estado (UF).
        /// </summary>
        public string Estado { get; set; } = string.Empty;
    }
}
