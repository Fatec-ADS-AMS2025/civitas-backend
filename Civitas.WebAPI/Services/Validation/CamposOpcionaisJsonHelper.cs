using System.Text.Json;

namespace Civitas.WebAPI.Services.Validation
{
    /// <summary>
    /// Helpers puros para serialização e validação dos campos opcionais dinâmicos (jsonb).
    /// </summary>
    public static class CamposOpcionaisJsonHelper
    {
        private const string EnvelopeKey = "camposOpcionais";
        private const int MaxFieldName = 100;
        private const int MaxFields = 50;

        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        /// <summary>
        /// Lê o envelope {"camposOpcionais":[...]} retornando os nomes declarados em ordem.
        /// Lança <see cref="JsonException"/> se o JSON for malformado.
        /// Lança <see cref="ArgumentException"/> se o envelope não tiver a chave esperada
        /// ou se algum nome for inválido (vazio, > 100 chars, duplicado).
        /// Retorna lista vazia se o input for null/whitespace.
        /// </summary>
        public static IReadOnlyList<string> ParseCamposDeclarados(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return Array.Empty<string>();
            }

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new ArgumentException("CamposOpcionais deve ser um objeto JSON.");
            }

            if (!doc.RootElement.TryGetProperty(EnvelopeKey, out var arr))
            {
                throw new ArgumentException($"CamposOpcionais deve conter a chave '{EnvelopeKey}'.");
            }

            if (arr.ValueKind != JsonValueKind.Array)
            {
                throw new ArgumentException($"'{EnvelopeKey}' deve ser um array de strings.");
            }

            var nomes = new List<string>();
            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var item in arr.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.String)
                {
                    throw new ArgumentException("Cada campo opcional deve ser uma string.");
                }

                var nome = item.GetString()?.Trim() ?? string.Empty;
                if (nome.Length == 0)
                {
                    throw new ArgumentException("Nome de campo opcional vazio não é permitido.");
                }

                if (nome.Length > MaxFieldName)
                {
                    throw new ArgumentException($"Nome '{nome}' excede {MaxFieldName} caracteres.");
                }

                if (!seen.Add(nome))
                {
                    throw new ArgumentException($"Nome de campo opcional duplicado: '{nome}'.");
                }

                nomes.Add(nome);
            }

            if (nomes.Count > MaxFields)
            {
                throw new ArgumentException($"Limite de {MaxFields} campos opcionais excedido.");
            }

            return nomes;
        }

        /// <summary>
        /// Constrói o envelope JSON {"camposOpcionais":[...]} a partir de uma lista de nomes.
        /// Retorna null se a lista estiver vazia/nula.
        /// Aplica as mesmas validações de <see cref="ParseCamposDeclarados"/>.
        /// </summary>
        public static string? SerializeCamposDeclarados(IEnumerable<string>? nomes)
        {
            if (nomes is null)
            {
                return null;
            }

            var lista = nomes.Where(n => !string.IsNullOrWhiteSpace(n)).Select(n => n.Trim()).ToList();
            if (lista.Count == 0)
            {
                return null;
            }

            var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var nome in lista)
            {
                if (nome.Length > MaxFieldName)
                {
                    throw new ArgumentException($"Nome '{nome}' excede {MaxFieldName} caracteres.");
                }

                if (!seen.Add(nome))
                {
                    throw new ArgumentException($"Nome de campo opcional duplicado: '{nome}'.");
                }
            }

            if (lista.Count > MaxFields)
            {
                throw new ArgumentException($"Limite de {MaxFields} campos opcionais excedido.");
            }

            var envelope = new Dictionary<string, IReadOnlyList<string>> { [EnvelopeKey] = lista };
            return JsonSerializer.Serialize(envelope, SerializerOptions);
        }

        /// <summary>
        /// Lê um objeto JSON plano {chave: valor, ...} retornando os pares como dicionário.
        /// Lança <see cref="JsonException"/> se malformado, <see cref="ArgumentException"/>
        /// se não for um objeto ou contiver chaves inválidas.
        /// Retorna dicionário vazio se input for null/whitespace.
        /// </summary>
        public static IReadOnlyDictionary<string, JsonElement> ParseValoresPreenchidos(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return new Dictionary<string, JsonElement>();
            }

            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.ValueKind != JsonValueKind.Object)
            {
                throw new ArgumentException("ValoresOpcionais deve ser um objeto JSON.");
            }

            var dict = new Dictionary<string, JsonElement>(StringComparer.Ordinal);
            foreach (var prop in doc.RootElement.EnumerateObject())
            {
                var nome = prop.Name?.Trim() ?? string.Empty;
                if (nome.Length == 0)
                {
                    throw new ArgumentException("Chave vazia não é permitida em ValoresOpcionais.");
                }

                if (dict.ContainsKey(nome))
                {
                    throw new ArgumentException($"Chave duplicada em ValoresOpcionais: '{nome}'.");
                }

                dict[nome] = prop.Value.Clone();
            }

            return dict;
        }

        /// <summary>
        /// Serializa um dicionário plano de valores opcionais em JSON.
        /// Retorna null se o dicionário for null ou vazio.
        /// </summary>
        public static string? SerializeValoresPreenchidos(IReadOnlyDictionary<string, JsonElement>? valores)
        {
            if (valores is null || valores.Count == 0)
            {
                return null;
            }

            return JsonSerializer.Serialize(valores, SerializerOptions);
        }

        /// <summary>
        /// Valida que todas as chaves de <paramref name="valoresChaves"/> existem em <paramref name="declarados"/>.
        /// Retorna lista de chaves desconhecidas (case-sensitive). Vazia = OK.
        /// </summary>
        public static IReadOnlyList<string> EncontrarChavesDesconhecidas(
            IEnumerable<string> valoresChaves,
            IEnumerable<string> declarados)
        {
            var declaradosSet = new HashSet<string>(declarados, StringComparer.Ordinal);
            return valoresChaves.Where(k => !declaradosSet.Contains(k)).ToList();
        }
    }
}
