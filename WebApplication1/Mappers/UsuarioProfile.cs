using AutoMapper;
using Meucombustivel.Dtos.Usuario;
using Meucombustivel.Models;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
        CreateMap<CreateUsuarioDto, Usuarios>()
            .ForMember(dest => dest.TipoUsuario, opt => opt.MapFrom(src => src.TipoUsuario))
            .ForMember(dest => dest.Id, opt => opt.Ignore())
         
            .ForMember(dest => dest.IdAuth, opt => opt.Ignore());

        CreateMap<UpdateUsuarioDto, Usuarios>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

        CreateMap<Usuarios, ReadUsuarioDto>().ReverseMap();
    }
}

