using AutoMapper;
using Meucombustivel.Dtos.Abastecimento;
using Meucombustivel.Models;


namespace Meucombustivel.Mappers
{


    /// <summary>
    /// Perfil de mapeamento AutoMapper para a entidade Abastecimento.
    /// Define as regras de conversão entre Abastecimento Model e  DTOs.
    /// </summary>
    public class AbastecimentoProfile : Profile
    {
        public AbastecimentoProfile()
        {
           
            CreateMap<CreateAbastecimentoDto, Abastecimento>()
                .ForMember(dest => dest.VeiculoId, opt => opt.MapFrom(src => src.VeiculoId));

            CreateMap<Abastecimento, ReadAbastecimentoDto>();

            CreateMap<UpdateAbastecimentoDto, Abastecimento>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) => srcMember != null)); 

        }
    }
}



