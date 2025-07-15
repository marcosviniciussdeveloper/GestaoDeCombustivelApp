using AutoMapper;
using Meucombustivel.Dtos.Empresa;
using Meucombustivel.Models;
// using Meucombustivel.Dtos.Usuario; // Este using não é estritamente necessário aqui para o mapeamento

namespace Meucombustivel.Mappers
{
    public class EmpresaProfile : Profile
    {
        public EmpresaProfile()
        {
            // Mapeamentos existentes
            CreateMap<CreateEmpresaDto, Empresa>();

            CreateMap<UpdateEmpresaDto, Empresa>()

                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Empresa, ReadEmpresaDto>().ReverseMap();

            CreateMap<RegisterCompanyAndAdminDto, Empresa>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .ForMember(dest => dest.RazaoSocial, opt => opt.MapFrom(src => src.RazaoSocial))
                .ForMember(dest => dest.Cnpj, opt => opt.MapFrom(src => src.Cnpj))
                .ForMember(dest => dest.NomeFantasia, opt => opt.MapFrom(src => src.NomeFantasia))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailEmpresa))
                .ForMember(dest => dest.Telefone, opt => opt.MapFrom(src => src.Telefone))
                .ForMember(dest => dest.Endereco, opt => opt.MapFrom(src => src.Endereco))
                // As propriedades AdminNome, AdminCpf, AdminEmail, AdminSenha
                // NÃO PERTENCEM à Model Empresa e, portanto, não devem ser mapeadas ou ignoradas aqui.
                // Elas serão usadas para criar a Model Usuarios separadamente no Serviço.
                ;
        }
        
    }
}