using AutoMapper;
using Civitas.WebAPI.DTOs;
using Civitas.WebAPI.Models;

namespace Civitas.WebAPI.Mappings;

public class SecretariaProfile : Profile
{
    public SecretariaProfile()
    {
        CreateMap<Secretaria, SecretariaDto>();
        CreateMap<SecretariaCreateDto, Secretaria>();
        CreateMap<SecretariaUpdateDto, Secretaria>()
            .ForMember(dest => dest.IdSecretaria, opt => opt.Ignore())
            .ForMember(dest => dest.DataCriacao, opt => opt.Ignore())
            .ForMember(dest => dest.Ativo, opt => opt.Ignore());
    }
}