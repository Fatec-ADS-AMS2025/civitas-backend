using AutoMapper;
using Civitas.WebAPI.Data.Interfaces;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Interfaces;

namespace Civitas.WebAPI.Services.Entities
{
    public class TipoDespesaService : GenericService<TipoDespesa, TipoDespesaDTO>, ITipoDespesaService
    {
        private readonly ITipoDespesaRepository _tipoDespesa;
        private readonly IMapper _mapper;

        public TipoDespesaService(ITipoDespesaRepository tipoDespesa, IMapper mapper)
            : base(tipoDespesa, mapper)
        {
            _tipoDespesa = tipoDespesa;
            _mapper = mapper;
        }

        public async Task<bool> ExisteUnidadesDeMedidaAtivas(int idTipoDespesa)
        {
            return await _tipoDespesa.ExisteUnidadesDeMedidaAtivas(idTipoDespesa);
        }
    }
}
