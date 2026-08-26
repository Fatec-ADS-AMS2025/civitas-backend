using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Data.Interfaces
{
    public interface IUnidadeMedidaRepository : IGenericRepository<UnidadeMedida>
    {
        Task<bool> ExistsByDescricaoNormalized(string descricaoNormalizada, int? ignoreId = null);
        Task<bool> ExistsByAbreviaturaNormalized(string abreviaturaNormalizada, int? ignoreId = null);
        Task<bool> HasTiposDespesaAtivosVinculados(int idUnidadeMedida);
    }
}
