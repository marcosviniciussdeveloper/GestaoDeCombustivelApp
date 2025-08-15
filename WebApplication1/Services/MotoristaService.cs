using Meucombustivel.Dtos.Motorista;
using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;
using Meucombustivel.Services.Interfaces;
using AutoMapper;
using Meucombustivel.Dtos.Usuario;
using Meucombustivel.Exceptions;
using Microsoft.AspNetCore.Razor.Hosting;

namespace Meucombustivel.Services
{
    public class MotoristaService : IMotoristaService
    {
        private readonly IMotoristaRepository _motoristaRepository;
        private readonly IMapper _mapper;
        private readonly IUsuarioService _usuarioService;
        private readonly IEmpresaRepository _empresaRepository;
        private readonly IUsuarioRepository _usuarioRepository;

        public MotoristaService(IEmpresaRepository empresaRepository, IMotoristaRepository motoristaRepository, IMapper mapper, IUsuarioRepository usuarioRepository, IUsuarioService usuarioService)
        {
            _motoristaRepository = motoristaRepository;
            _mapper = mapper;
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _empresaRepository = empresaRepository;
        }

        public async Task<Guid> CreateAsync(Guid usuarioId, CreateMotoristaDto dto)
        {
            var existingUser = await _usuarioRepository.GetByIdAsync(usuarioId);
            if (existingUser == null)
            {
                throw new NotFoundException($"Usuário com ID {usuarioId} não encontrado. Não é possível criar perfil de motorista.");
            }

            var existingMotoristaProfile = await _motoristaRepository.GetByUsuarioIdAsync(usuarioId);
            if (existingMotoristaProfile != null)
            {
                throw new BusinessException($"O usuário com ID {usuarioId} já possui um perfil de motorista.");
            }

            var motorista = _mapper.Map<Motorista>(dto);
            motorista.UsuarioId = usuarioId;

            if (existingUser.TipoUsuario != "motorista")
            {
                existingUser.TipoUsuario = "motorista";
                await _usuarioRepository.UpdateAsync(existingUser);
            }

            await _motoristaRepository.AddAsync(motorista);

            return usuarioId;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingMotoristaProfile = await _motoristaRepository.GetByIdAsync(id);
            if (existingMotoristaProfile == null)
            {
                throw new NotFoundException($"Motorista com ID {id} não encontrado para exclusão.");
            }

            await _motoristaRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ReadMotoristaDto>> GetAllAsync()
        {
            var motoristas = await _motoristaRepository.GetAllAsync();
            var motoristaDtos = new List<ReadMotoristaDto>();

            foreach (var motorista in motoristas)
            {
                var usuario = await _usuarioRepository.GetByIdAsync(motorista.UsuarioId);
                var motoristaDto = _mapper.Map<ReadMotoristaDto>(motorista);
                if (usuario != null)
                {
                    motoristaDto.Nome = usuario.Nome;
                    motoristaDto.Cpf = usuario.Cpf;
                    motoristaDto.Email = usuario.Email;
                }
                motoristaDtos.Add(motoristaDto);
            }
            return motoristaDtos;
        }

        public async Task<IEnumerable<ReadMotoristaDto>> GetAllByEmpresaAsync(Guid empresaId)
        {
            var empresa = await _empresaRepository.GetByIdAsync(empresaId);


            if (empresa == null)
                throw new NotFoundException($"Empresa com ID {empresaId} não encontrada.");

            var motoristas = await _motoristaRepository.GetAllAsync();
            var motoristaDtos = new List<ReadMotoristaDto>();

            foreach (var motorista in motoristas)
            {
                var usuario = await _usuarioRepository.GetByIdAsync(motorista.UsuarioId);


                if (usuario != null && usuario.EmpresaId == empresa.Id)
                {
                    var motoristaDto = _mapper.Map<ReadMotoristaDto>(motorista);
                    motoristaDto.Nome = usuario.Nome;
                    motoristaDto.Cpf = usuario.Cpf;
                    motoristaDto.Email = usuario.Email;
                    motoristaDtos.Add(motoristaDto);
                }
            }

            return motoristaDtos;
        }

        public async Task<ReadMotoristaDto?> GetByIdAsync(Guid id)
        {
            var motorista = await _motoristaRepository.GetByIdAsync(id);
            if (motorista == null)
                throw new NotFoundException($"Motorista com ID {id} não encontrado.");


            var usuario = await _usuarioRepository.GetByIdAsync(motorista.UsuarioId);


            var motoristaDto = _mapper.Map<ReadMotoristaDto>(motorista);


            if (usuario != null)
            {
                motoristaDto.Nome = usuario.Nome;
                motoristaDto.Email = usuario.Email;
                motoristaDto.Cpf = usuario.Cpf;
            }


            return motoristaDto;
        }



        public async Task<ReadMotoristaDto?> GetByUsuarioIdAsync(Guid usuarioId)
        {
            var motorista = await _motoristaRepository.GetByUsuarioIdAsync(usuarioId);
            if (motorista == null) return null;

            var dto = _mapper.Map<ReadMotoristaDto>(motorista);

           
            var usuario = await _usuarioRepository.GetByIdAsync(motorista.UsuarioId);
            if (usuario != null)
            {
                dto.Nome = usuario.Nome;
                dto.Email = usuario.Email;
                dto.Cpf = usuario.Cpf;
            }

            return dto;
        }



        public async Task<Guid> ResgisterNewDriverAsync(RegisterMotoristaDto dto)
        {
            var existingUser = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new BusinessException($"Já existe um usuário com o e-mail {dto.Email}.");

            var existingUserCpf = await _usuarioRepository.GetByCpfAsync(dto.Cpf);
            if (existingUserCpf != null)
                throw new BusinessException($"Já existe um usuário com o CPF {dto.Cpf}.");

            var userCreateDto = new CreateUsuarioDto
            {
                Nome = dto.Nome,
                Cpf = dto.Cpf,
                Email = dto.Email,
                Senha = dto.Senha,
                TipoUsuario = "motorista",
                EmpresaId = dto.EmpresaId
            };

            Guid newUsuarioId = await _usuarioService.CreateAsync(userCreateDto);


            var motorista = _mapper.Map<Motorista>(dto);
            motorista.UsuarioId = newUsuarioId;


            var criado = await _motoristaRepository.AddAsync(motorista);


            if (dto.EmpresaId.HasValue && dto.EmpresaId.Value != Guid.Empty)
            {

            }


            return newUsuarioId;
        }



        public async Task<bool> UpdateAsync(Guid id, UpdateMotoristaDto dto)
        {
            var existingMotoristaProfile = await _motoristaRepository.GetByIdAsync(id);
            if (existingMotoristaProfile == null)
            {
                throw new NotFoundException($"Motorista com ID {id} não encontrado para atualização.");
            }

            var userUpdateDto = new UpdateUsuarioDto
            {
                Nome = dto.Nome,
                Cpf = dto.Cpf,
                Email = dto.Email,
            };

            await _usuarioService.UpdateAsync(id, userUpdateDto);

            _mapper.Map(dto, existingMotoristaProfile);
            await _motoristaRepository.UpdateAsync(existingMotoristaProfile);
            return true;
        }
    }
}
