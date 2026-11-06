using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>Factory de <see cref="UnidadeMedida"/>.</summary>
    public class UnidadeMedidaFactory : Factory<UnidadeMedida, UnidadeMedidaFactory>
    {
        private static readonly (string Descricao, string Abreviatura)[] Unidades =
        [
            ("Quilowatt-hora", "kWh"), ("Metro cubico", "m3"), ("Litro", "L"), ("Unidade", "un"),
            ("Quilograma", "kg"), ("Hora", "h"), ("Megabit por segundo", "Mbps")
        ];

        protected override UnidadeMedida Definition()
        {
            var (descricao, abreviatura) = Faker.PickRandom(Unidades);
            return new UnidadeMedida(0, descricao, abreviatura, Situacao.ATIVO);
        }

        public UnidadeMedidaFactory Inativa() => State(u => u.Situacao = Situacao.INATIVO);
    }
}
