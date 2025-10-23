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

    }
}
