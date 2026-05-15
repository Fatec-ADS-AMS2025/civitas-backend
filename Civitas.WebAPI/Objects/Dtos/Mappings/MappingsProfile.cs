using AutoMapper;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Validation;

namespace Civitas.WebAPI.Objects.Dtos.Mappings
{
    public class MappingsProfile : Profile
    {
        public MappingsProfile()
        {
            CreateMap<Usuario, UsuarioDTO>()
                .ForMember(dest => dest.Senha, opt => opt.Ignore());
            CreateMap<UsuarioDTO, Usuario>();
            CreateMap<Fornecedor, FornecedorDTO>().ReverseMap();
            CreateMap<Secretaria, SecretariaDTO>().ReverseMap();
            CreateMap<Auditoria, AuditoriaDTO>().ReverseMap();
            CreateMap<TipoInstituicao, TipoInstituicaoDTO>().ReverseMap();
            CreateMap<Instituicao, InstituicaoDTO>().ReverseMap();
            CreateMap<UnidadeMedida, UnidadeMedidaDTO>().ReverseMap();

            CreateMap<TipoDespesa, TipoDespesaDTO>()
                .ForMember(dest => dest.CamposOpcionais, opt => opt.MapFrom(src =>
                    ParseCamposSafely(src.CamposOpcionais)));
            CreateMap<TipoDespesaDTO, TipoDespesa>()
                .ForMember(dest => dest.CamposOpcionais, opt => opt.MapFrom(src =>
                    CamposOpcionaisJsonHelper.SerializeCamposDeclarados(src.CamposOpcionais)));

            CreateMap<Orcamento, OrcamentoDTO>().ReverseMap();
            CreateMap<Despesa, DespesaDTO>().ReverseMap();

            CreateMap<TipoCodigo, TipoCodigoDTO>().ReverseMap();
            CreateMap<UnidadeConsumidora, UnidadeConsumidoraDTO>().ReverseMap();
        }

        private static IList<string>? ParseCamposSafely(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return CamposOpcionaisJsonHelper.ParseCamposDeclarados(json).ToList();
            }
            catch
            {
                return null;
            }
        }

    }
}
