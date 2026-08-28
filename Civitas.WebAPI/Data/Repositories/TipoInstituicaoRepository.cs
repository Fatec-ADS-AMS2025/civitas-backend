using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Enums;
using Civitas.WebAPI.Objects.Models;
using Microsoft.EntityFrameworkCore;

namespace Civitas.WebAPI.Data.Repositories
{
    public class TipoInstituicaoRepository : GenericRepository<TipoInstituicao>, ITipoInstituicaoRepository
    {
        private readonly AppDbContext _appDbContext;

        public TipoInstituicaoRepository(AppDbContext context) : base(context)
        {
            _appDbContext = context;
        }

        public async Task<bool> ExisteInstituicoesAtivasAsync(int idTipoInstituicao)
        {
            return await _appDbContext.Instituicoes
                .AnyAsync(i => i.IdTipoInstituicao == idTipoInstituicao && i.Situacao == Situacao.ATIVO);
        }

        public async Task<bool> ExistsByDescricaoNormalized(string descricaoNormalizada, int? ignoreId = null)
        {
            var query = _appDbContext.TipoInstituicoes
                .AsNoTracking()
                .Where(t => !t.Excluido && t.Descricao.Trim().ToUpper() == descricaoNormalizada);

            if (ignoreId.HasValue)
            {
                query = query.Where(t => t.Id != ignoreId.Value);
            }

            return await query.AnyAsync();
        }

    }
}
