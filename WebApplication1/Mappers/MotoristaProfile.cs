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
                .ForMember(d => d.MotoristaId,  o => o.MapFrom(s => s.UsuarioId)) // <- nome certo
                .ForMember(d => d.Nome,         o => o.Ignore()) 
                .ForMember(d => d.Email,        o => o.Ignore())  
                .ForMember(d => d.Cpf,          o => o.Ignore())  
                .ForMember(d => d.StatusVinculo,o => o.Ignore()); 
         
            CreateMap<RegisterMotoristaDto, Motorista>()
                .ForMember(d => d.UsuarioId,    o => o.Ignore())
                .ForMember(d => d.NumeroCnh,    o => o.MapFrom(s => s.NumeroCnh))
                .ForMember(d => d.CategoriaCnh, o => o.MapFrom(s => s.CategoriaCnh))
                .ForMember(d => d.ValidadeCnh,  o => o.MapFrom(s => s.ValidadeCnh));
        }
    }
}
