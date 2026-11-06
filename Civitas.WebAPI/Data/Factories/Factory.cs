using Bogus;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>
    /// Base das factories de modelo, no estilo das factories do Laravel.
    /// </summary>
    /// <remarks>
    /// Cada factory declara o estado padrão de um modelo em <see cref="Definition"/> e pode expor
    /// estados nomeados (ex.: <c>UsuarioFactory.New().Administrador()</c>) que aplicam ajustes sobre ele.
    /// <para>
    /// <c>Make</c> apenas instancia (sem tocar no banco); <c>CreateAsync</c> instancia e persiste.
    /// Relacionamentos obrigatórios são criados automaticamente pela definição, a menos que um estado
    /// <c>Para...(entidade)</c> informe uma entidade existente.
    /// </para>
    /// </remarks>
    /// <typeparam name="TModel">Modelo produzido.</typeparam>
    /// <typeparam name="TSelf">A própria factory concreta (permite encadear estados).</typeparam>
    public abstract class Factory<TModel, TSelf>
        where TModel : class
        where TSelf : Factory<TModel, TSelf>, new()
    {
        private static int _sequence;
        private readonly List<Action<TModel>> _states = [];

        /// <summary>Gerador de dados falsos em português do Brasil.</summary>
        protected Faker Faker { get; } = new("pt_BR");

        /// <summary>Cria uma nova instância da factory.</summary>
        public static TSelf New() => new();

        /// <summary>Estado padrão do modelo.</summary>
        protected abstract TModel Definition();

        /// <summary>
        /// Próximo número de uma sequência por modelo. Use para garantir unicidade
        /// em campos com índice único (e-mail, matrícula, identificador...).
        /// </summary>
        protected static int NextSequence() => Interlocked.Increment(ref _sequence);

        /// <summary>Aplica um ajuste ao modelo depois da definição padrão.</summary>
        public TSelf State(Action<TModel> state)
        {
            _states.Add(state);
            return (TSelf)this;
        }

        /// <summary>Instancia um modelo sem persistir.</summary>
        public TModel Make()
        {
            var model = Definition();
            foreach (var state in _states)
            {
                state(model);
            }

            return model;
        }

        /// <summary>Instancia <paramref name="count"/> modelos sem persistir.</summary>
        public List<TModel> Make(int count) => Enumerable.Range(0, count).Select(_ => Make()).ToList();

        /// <summary>Instancia e persiste um modelo (incluindo relacionamentos ainda não salvos).</summary>
        public async Task<TModel> CreateAsync(AppDbContext context, CancellationToken cancellationToken = default)
        {
            var model = Make();
            await context.Set<TModel>().AddAsync(model, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return model;
        }

        /// <summary>Instancia e persiste <paramref name="count"/> modelos.</summary>
        public async Task<List<TModel>> CreateAsync(AppDbContext context, int count, CancellationToken cancellationToken = default)
        {
            var models = Make(count);
            await context.Set<TModel>().AddRangeAsync(models, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);
            return models;
        }

        /// <summary>Somente dígitos, no tamanho informado.</summary>
        protected string Digits(int length) => Faker.Random.ReplaceNumbers(new string('#', length));

        /// <summary>CPF válido (com dígitos verificadores), apenas dígitos.</summary>
        protected string Cpf() => WithCheckDigits(Digits(9), [10, 9, 8, 7, 6, 5, 4, 3, 2], [11, 10, 9, 8, 7, 6, 5, 4, 3, 2]);

        /// <summary>CNPJ válido (com dígitos verificadores, filial 0001), apenas dígitos.</summary>
        protected string Cnpj() => WithCheckDigits(Digits(8) + "0001", [5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2], [6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2]);

        private static string WithCheckDigits(string baseDigits, int[] firstWeights, int[] secondWeights)
        {
            static int CheckDigit(string digits, int[] weights)
            {
                var sum = digits.Select((c, i) => (c - '0') * weights[i]).Sum();
                var rest = sum % 11;
                return rest < 2 ? 0 : 11 - rest;
            }

            var withFirst = baseDigits + CheckDigit(baseDigits, firstWeights);
            return withFirst + CheckDigit(withFirst, secondWeights);
        }

        /// <summary>Sigla de UF aleatória.</summary>
        protected string Uf() => Faker.Address.StateAbbr();

        /// <summary>Telefone fixo/celular com DDD, apenas dígitos.</summary>
        protected string Telefone() => Faker.Random.ReplaceNumbers("41#########")[..Faker.Random.Int(10, 11)];
    }
}
