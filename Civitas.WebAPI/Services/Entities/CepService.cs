using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Services.Interfaces;
using Civitas.WebAPI.Services.Validation;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Civitas.WebAPI.Services.Entities
{
    /// <summary>
    /// Serviço responsável pela consulta de endereços na API pública ViaCEP.
    /// </summary>
    /// <remarks>
    /// Este serviço é desacoplado das entidades do sistema e pode ser reutilizado
    /// por qualquer módulo que precise preencher endereço a partir do CEP.
    /// </remarks>
    public class CepService : ICepService
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private readonly HttpClient _httpClient;

        /// <summary>
        /// Inicializa o serviço de consulta de CEP.
        /// </summary>
        /// <param name="httpClient">Cliente HTTP configurado via DI.</param>
        public CepService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        /// <inheritdoc />
        public async Task<EnderecoViaCepDTO?> BuscarEnderecoPorCepAsync(string cep)
        {
            var cepSanitizado = SanitizarCep(cep);

            if (cepSanitizado.Length != 8)
            {
                throw new ArgumentException("CEP deve conter exatamente 8 dígitos numéricos.");
            }

            try
            {
                using var response = await _httpClient.GetAsync($"ws/{cepSanitizado}/json/");
                response.EnsureSuccessStatusCode();

                await using var contentStream = await response.Content.ReadAsStreamAsync();
                var viaCepResponse = await JsonSerializer.DeserializeAsync<ViaCepResponse>(contentStream, JsonOptions);

                if (viaCepResponse?.Erro == true)
                {
                    return null;
                }

                if (viaCepResponse is null)
                {
                    throw new CepServiceException("Resposta inválida ao consultar o serviço de CEP.");
                }

                return new EnderecoViaCepDTO
                {
                    Cep = viaCepResponse.Cep,
                    Logradouro = viaCepResponse.Logradouro,
                    Bairro = viaCepResponse.Bairro,
                    Cidade = viaCepResponse.Cidade,
                    Estado = viaCepResponse.Estado
                };
            }
            catch (ArgumentException)
            {
                throw;
            }
            catch (TaskCanceledException ex)
            {
                throw new CepServiceException("Tempo limite excedido ao consultar o CEP.", ex);
            }
            catch (HttpRequestException ex)
            {
                throw new CepServiceException("Falha ao consultar o serviço de CEP.", ex);
            }
            catch (JsonException ex)
            {
                throw new CepServiceException("Falha ao processar a resposta do serviço de CEP.", ex);
            }
        }

        private static string SanitizarCep(string? cep)
        {
            if (string.IsNullOrWhiteSpace(cep))
            {
                return string.Empty;
            }

            return Regex.Replace(cep, "[^0-9]", string.Empty);
        }

        private sealed class ViaCepResponse
        {
            [System.Text.Json.Serialization.JsonPropertyName("cep")]
            public string Cep { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("logradouro")]
            public string Logradouro { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("bairro")]
            public string Bairro { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("localidade")]
            public string Cidade { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("uf")]
            public string Estado { get; set; } = string.Empty;

            [System.Text.Json.Serialization.JsonPropertyName("erro")]
            public bool? Erro { get; set; }
        }
    }
}
