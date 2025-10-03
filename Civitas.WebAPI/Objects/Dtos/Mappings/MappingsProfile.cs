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
        }
    }
}
