
using AutoMapper;
using Meucombustivel.Dtos.Usuario;
using Meucombustivel.Models;

public class UsuarioProfile : Profile
{
    public UsuarioProfile()
    {
       
        CreateMap<Usuarios, ReadUsuarioDto>()
           
            .ForMember(dest => dest.EmpresaId, opt => opt.MapFrom(src => src.EmpresaId));

   
        CreateMap<CreateUsuarioDto, Usuarios>()
            .ForMember(d => d.TipoUsuario, o => o.MapFrom(s =>
                string.IsNullOrWhiteSpace(s.TipoUsuario) ? "usuario" : s.TipoUsuario))
            .ForMember(d => d.Id, o => o.Ignore())
            .ForMember(d => d.IdAuth, o => o.Ignore());

      
        CreateMap<UpdateUsuarioDto, Usuarios>()
            .ForAllMembers(o => o.Condition((src, dest, srcMember) => srcMember != null));
    }
}
