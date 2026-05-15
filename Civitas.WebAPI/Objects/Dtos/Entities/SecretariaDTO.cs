using Civitas.WebAPI.Objects.Enums;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// Objeto de transferência de dados para operações envolvendo Secretarias (Órgãos Gestores).
    /// </summary>
    /// <remarks>
    /// Finalidade:
    /// - Input: Receber dados para cadastro/edição de uma secretaria via formulário.
    /// - Output: Retornar os detalhes da secretaria para visualização.
    /// </remarks>
    public class SecretariaDTO
    {
        /// <summary>
        /// Identificador único da secretaria.
        /// </summary>
        /// <remarks>
        /// Input: Ignorar na criação. Obrigatório na atualização.
        /// </remarks>
        public int IdSecretaria { get; set; }

        /// <summary>
        /// Situação cadastral.
        /// </summary>
        /// <remarks>
        /// Valores: <see cref="Situacao"/>. 
        /// Secretarias inativas bloqueiam a criação de novas instituições vinculadas.
        /// </remarks>
        public Situacao Situacao { get; set; }

        /// <summary>
        /// Descrição detalhada ou área de atuação.
        /// </summary>
        public string Descricao { get; set; } = string.Empty;

        /// <summary>
        /// CNPJ da secretaria.
        /// </summary>
        /// <remarks>
        /// Obrigatório. Deve ser enviado sem máscara (apenas números) ou validado pelo backend se vier com pontuação.
        /// </remarks>
        public string Cnpj { get; set; } = string.Empty;

        /// <summary>
        /// Nome Fantasia (Nome de exibição).
        /// </summary>
        /// <example>Secretaria de Obras, Secretaria da Saúde.</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Logradouro (Rua/Av) da sede.
        /// </summary>
        public string Logradouro { get; set; } = string.Empty;

        /// <summary>
        /// Número do endereço.
        /// </summary>
        public string Numero { get; set; } = string.Empty;

        /// <summary>
        /// Bairro.
        /// </summary>
        public string Bairro { get; set; } = string.Empty;

        /// <summary>
        /// Código Postal (CEP).
        /// </summary>
        public string Cep { get; set; } = string.Empty;

        /// <summary>
        /// Razão Social oficial (Nome Jurídico).
        /// </summary>
        public string NomeRazaoSocial { get; set; } = string.Empty;

        /// <summary>
        /// E-mail oficial de contato.
        /// </summary>
        /// <remarks>
        /// O sistema deve validar o formato de e-mail.
        /// </remarks>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Telefone de contato.
        /// </summary>
        public string Telefone { get; set; } = string.Empty;

        /// <summary>
        /// Cidade da sede.
        /// </summary>
        public string Cidade { get; set; } = string.Empty;

        /// <summary>
        /// Estado (UF).
        /// </summary>
        public string Estado { get; set; } = string.Empty;
        public bool Excluido { get; set; }

        public DateTime? DataExclusao { get; set; }

    }
}
