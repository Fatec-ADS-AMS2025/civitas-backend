using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Factories
{
    /// <summary>Factory de <see cref="TipoCodigo"/>.</summary>
    public class TipoCodigoFactory : Factory<TipoCodigo, TipoCodigoFactory>
    {
        protected override TipoCodigo Definition()
        {
            return new TipoCodigo(
                0,
                $"CODIGO-{NextSequence():D3}",
                Faker.Lorem.Sentence(6));
        }
    }
}
