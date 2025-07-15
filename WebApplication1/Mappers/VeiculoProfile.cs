using AutoMapper;
using Meucombustivel.Dtos.Veiculo;
using Meucombustivel.Models;

// <summary>
// Mapper para a entidade Veiculo, configurando os mapeamentos entre modelos e DTOs.
// </summary>


namespace Meucombustivel.Mappers
{
    public class VeiculoProfile : Profile
    {

       public VeiculoProfile()
        { 

            CreateMap<CreateVeiculoDto , Veiculo>();

            CreateMap<UpdateVeiculoDto, Veiculo>()
               .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Veiculo, ReadVeiculoDto>() .ReverseMap();

        }
    }
}
