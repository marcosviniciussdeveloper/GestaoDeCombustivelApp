using AutoMapper;
using Meucombustivel.Dtos.Motorista;
using Meucombustivel.Models;

namespace Meucombustivel.Mappers
{
    public class MotoristaProfile : Profile
    {
        public MotoristaProfile()
        {
           
            CreateMap<CreateMotoristaDto, Motorista>();

            CreateMap<UpdateMotoristaDto, Motorista>()
                 .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null));
           
            CreateMap<Motorista, ReadMotoristaDto>()
    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.UsuarioId));

           
            CreateMap<RegisterMotoristaDto, Motorista>()
                .ForMember(dest => dest.UsuarioId, opt => opt.Ignore()) 
                .ForMember(dest => dest.NumeroCnh, opt => opt.MapFrom(src => src.NumeroCnh))
                .ForMember(dest => dest.CategoriaCnh, opt => opt.MapFrom(src => src.CategoriaCnh))
                .ForMember(dest => dest.ValidadeCnh, opt => opt.MapFrom(src => src.ValidadeCnh));
               
        }
    }
}
