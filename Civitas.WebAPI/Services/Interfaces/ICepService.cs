using Civitas.WebAPI.Objects.Dtos.Entities;

namespace Civitas.WebAPI.Services.Interfaces
{
    /// <summary>
    /// Contrato para consulta externa de endereços por CEP.
    /// </summary>
    public interface ICepService
    {
        /// <summary>
        /// Consulta a API ViaCEP e retorna os dados de endereço correspondentes ao CEP informado.
        /// </summary>
        /// <param name="cep">CEP informado pelo cliente, com ou sem máscara.</param>
        /// <returns>
        /// Dados do endereço quando o CEP existe; <c>null</c> quando a ViaCEP informa que o CEP não foi encontrado.
        /// </returns>
        Task<EnderecoViaCepDTO?> BuscarEnderecoPorCepAsync(string cep);
    }
}
