using System.Text.Json.Serialization;

namespace Civitas.WebAPI.Objects.Dtos.Entities
{
    /// <summary>
    /// DTO utilizado para representar o endereço retornado pela API ViaCEP.
    /// </summary>
    /// <remarks>
    /// Este objeto é reutilizável por qualquer formulário que precise consultar endereço por CEP
    /// sem depender de entidades persistidas do sistema.
    /// </remarks>
    public class EnderecoViaCepDTO
    {
        /// <summary>
        /// Código de Endereçamento Postal consultado.
        /// </summary>
        public string Cep { get; set; } = string.Empty;

        /// <summary>
        /// Nome do logradouro.
        /// </summary>
        public string Logradouro { get; set; } = string.Empty;

        /// <summary>
        /// Nome do bairro.
        /// </summary>
        public string Bairro { get; set; } = string.Empty;

        /// <summary>
        /// Cidade retornada pela ViaCEP.
        /// </summary>
        public string Cidade { get; set; } = string.Empty;

        /// <summary>
        /// Unidade Federativa retornada pela ViaCEP.
        /// </summary>
        public string Estado { get; set; } = string.Empty;

        /// <summary>
        /// Indicador retornado pela ViaCEP quando o CEP não é encontrado.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public bool? Erro { get; set; }
    }
}
