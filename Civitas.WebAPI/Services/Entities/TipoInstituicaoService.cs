using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class TipoInstituicaoService : GenericService<TipoInstituicao, TipoInstituicaoDTO>, ITipoInstituicaoService
    {
        private readonly ITipoInstituicaoRepository _tipoInstituicao;
        private readonly IMapper _mapper;

        public TipoInstituicaoService(ITipoInstituicaoRepository tipoInstituicao, IMapper mapper)
            : base(tipoInstituicao, mapper)
        {
            _tipoInstituicao = tipoInstituicao;
            _mapper = mapper;
        }

        public async Task<bool> ExisteInstituicoesAtivas(int idTipoInstituicao)
        {
            return await _tipoInstituicao.ExisteInstituicoesAtivasAsync(idTipoInstituicao);
        }

    }
}
