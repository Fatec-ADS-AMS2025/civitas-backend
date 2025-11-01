using AutoMapper;
using Civitas.WebAPI.Objects.Dtos.Entities;
using Civitas.WebAPI.Objects.Models;

namespace Civitas.WebAPI.Objects.Dtos.Mappings
{
    public class MappingsProfile : Profile
    {

        public MappingsProfile()
        {
            CreateMap<Usuario, UsuarioDTO>().ReverseMap();
            CreateMap<Fornecedor, FornecedorDTO>().ReverseMap();
            CreateMap<Secretaria, SecretariaDTO>().ReverseMap();
            CreateMap<Documento, DocumentoDTO>().ReverseMap();
            CreateMap<Auditoria, AuditoriaDTO>().ReverseMap();
            CreateMap<TipoInstituicao, TipoInstituicaoDTO>().ReverseMap();
            CreateMap<Instituicao, InstituicaoDTO>().ReverseMap();
            CreateMap<UnidadeMedida, UnidadeMedidaDTO>().ReverseMap();
            CreateMap<TipoDespesa, TipoDespesaDTO>().ReverseMap();
        }
    }
}
