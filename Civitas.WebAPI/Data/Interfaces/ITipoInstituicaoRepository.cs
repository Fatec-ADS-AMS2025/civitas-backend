using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface ITipoInstituicaoRepository : IGenericRepository<TipoInstituicao>
    {
        Task<bool> ExisteInstituicoesAtivasAsync(int idTipoInstituicao);
        Task<bool> ExistsByDescricaoNormalized(string descricaoNormalizada, int? ignoreId = null);
    }
}
