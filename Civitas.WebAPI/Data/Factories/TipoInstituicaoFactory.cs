using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>Factory de <see cref="TipoInstituicao"/>.</summary>
    public class TipoInstituicaoFactory : Factory<TipoInstituicao, TipoInstituicaoFactory>
    {
        private static readonly string[] Tipos =
            ["Escola Municipal", "Centro de Educacao Infantil", "Unidade Basica de Saude", "Biblioteca", "Ginasio", "Centro Cultural"];

        protected override TipoInstituicao Definition()
        {
            return new TipoInstituicao(0, Faker.PickRandom(Tipos), Situacao.ATIVO);
        }

        public TipoInstituicaoFactory Inativo() => State(t => t.Situacao = Situacao.INATIVO);
    }
}
