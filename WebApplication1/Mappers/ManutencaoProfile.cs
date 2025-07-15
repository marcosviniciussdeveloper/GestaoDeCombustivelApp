using AutoMapper;
using Meucombustivel.Dtos.Manutencao;
using Meucombustivel.Models;

namespace Meucombustivel.Mappers
{
    public class ManutencaoProfile : Profile
    {

        public ManutencaoProfile()
        {

            CreateMap<CreateManutencaoDto, Manutencoes>()
                .ForMember(dest => dest.IdVeiculo , opt => opt.MapFrom(src => src.IdVeiculo));


            CreateMap<UpdateManutencaoDto, Manutencoes>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); 

            CreateMap<Manutencoes, ReadManutencaoDto>() .ReverseMap();

        }
    }
}
