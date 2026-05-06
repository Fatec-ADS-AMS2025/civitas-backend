using AutoMapper;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;
using Civitas.WebAPI.Services.Validation;
using System.Text.Json;

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
            CreateMap<Documento, DocumentoDTO>().ReverseMap();
            CreateMap<Auditoria, AuditoriaDTO>().ReverseMap();
            CreateMap<TipoInstituicao, TipoInstituicaoDTO>().ReverseMap();
            CreateMap<Instituicao, InstituicaoDTO>().ReverseMap();
            CreateMap<Fluxo, FluxoDTO>().ReverseMap();
            CreateMap<UnidadeMedida, UnidadeMedidaDTO>().ReverseMap();

            CreateMap<TipoDespesa, TipoDespesaDTO>()
                .ForMember(dest => dest.CamposOpcionais, opt => opt.MapFrom(src =>
                    ParseCamposSafely(src.CamposOpcionais)));
            CreateMap<TipoDespesaDTO, TipoDespesa>()
                .ForMember(dest => dest.CamposOpcionais, opt => opt.MapFrom(src =>
                    CamposOpcionaisJsonHelper.SerializeCamposDeclarados(src.CamposOpcionais)));

            CreateMap<Orcamento, OrcamentoDTO>().ReverseMap();

            CreateMap<Despesa, DespesaDTO>()
                .ForMember(dest => dest.ValoresOpcionais, opt => opt.MapFrom(src =>
                    ParseValoresSafely(src.ValoresOpcionais)));
            CreateMap<DespesaDTO, Despesa>()
                .ForMember(dest => dest.ValoresOpcionais, opt => opt.MapFrom(src =>
                    SerializeValoresOpcionais(src.ValoresOpcionais)));

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

        private static IDictionary<string, JsonElement>? ParseValoresSafely(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
            {
                return null;
            }

            try
            {
                return CamposOpcionaisJsonHelper.ParseValoresPreenchidos(json)
                    .ToDictionary(kv => kv.Key, kv => kv.Value);
            }
            catch
            {
                return null;
            }
        }

        private static string? SerializeValoresOpcionais(IDictionary<string, JsonElement>? valores)
        {
            if (valores == null)
            {
                return null;
            }

            IReadOnlyDictionary<string, JsonElement> readOnly =
                valores as IReadOnlyDictionary<string, JsonElement>
                ?? new Dictionary<string, JsonElement>(valores);

            return CamposOpcionaisJsonHelper.SerializeValoresPreenchidos(readOnly);
        }
    }
}
