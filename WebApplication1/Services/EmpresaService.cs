using AutoMapper;
using Meucombustivel.Dtos.Empresa;
using Meucombustivel.Dtos.Usuario;
using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;
using Meucombustivel.Services.Interfaces;


namespace Meucombustivel.Services
{
    public class EmpresaService : IEmpresaService
    {
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IUsuarioService _usuarioService; 
        private readonly IMapper _mapper;

        public EmpresaService(IEmpresaRepository empresaRepository, IUsuarioService usuarioService, IMapper mapper)
        {
            _empresaRepository = empresaRepository;
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        public async Task<Guid> CreateAsync(CreateEmpresaDto dto)
        {
            var existingCompany = await _empresaRepository.GetByCnpjAsync(dto.Cnpj); 
            if (existingCompany != null)
            {
                throw new InvalidOperationException($"Já existe uma empresa com o CNPJ {dto.Cnpj}.");
            }

            var empresa = _mapper.Map<Empresa>(dto);
            empresa.Id = Guid.NewGuid();
            await _empresaRepository.AddAsync(empresa);

            return empresa.Id;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingCompany = await _empresaRepository.GetByIdAsync(id); 
            if (existingCompany == null)
            {
                return false;
            }

            await _empresaRepository.DeleteAsync(id);
            return true;
        }


        public async Task<IEnumerable<ReadEmpresaDto>> GetAllAsync()
        {
            var empresas = await _empresaRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReadEmpresaDto>>(empresas);
        }

    
        public async Task<ReadEmpresaDto?> GetByIdAsync(Guid id)
        {
            var empresa = await _empresaRepository.GetByIdAsync(id); 
            if (empresa == null)
            {
                return null; 
            }
            return _mapper.Map<ReadEmpresaDto>(empresa);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateEmpresaDto dto)
        {
            var updateCompany = await _empresaRepository.GetByIdAsync(id); 
            if (updateCompany == null)
            {
                return false;
            }
            
            if (!string.IsNullOrEmpty(dto.Cnpj) && dto.Cnpj != updateCompany.Cnpj)
            {
                var existingEmpresa = await _empresaRepository.GetByCnpjAsync(dto.Cnpj); 
                if (existingEmpresa != null && existingEmpresa.Id != id)
                {
                    throw new InvalidOperationException($"Já existe uma empresa com o CNPJ {dto.Cnpj}.");
                }
            }

            _mapper.Map(dto, updateCompany); 
            await _empresaRepository.UpdateAsync(updateCompany); 
            return true;
        }

       
        public async Task<ReadEmpresaDto> RegisterCompanyAndAdminAsync(RegisterCompanyAndAdminDto dto)
        {
            var existingCompany = await _empresaRepository.GetByCnpjAsync(dto.Cnpj);
            if (existingCompany != null)
            {
                throw new InvalidOperationException($"Já existe uma empresa com o CNPJ {dto.Cnpj}.");
            }

            var empresa = _mapper.Map<Empresa>(dto);
        
         
            var createdEmpresa = await _empresaRepository.AddAsync(empresa); 

            var createAdminUserDto = new CreateUsuarioDto
            {
                Nome = dto.AdminNome,
                Cpf = dto.AdminCpf,
                Email = dto.AdminEmail,
                Senha = dto.AdminSenha,
                TipoUsuario = "administrador",
                EmpresaId = createdEmpresa.Id 
            };

            Guid adminUsuarioId = await _usuarioService.CreateAsync(createAdminUserDto);

            return _mapper.Map<ReadEmpresaDto>(createdEmpresa); 
        }
    }
}

