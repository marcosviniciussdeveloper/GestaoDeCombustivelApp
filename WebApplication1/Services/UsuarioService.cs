using AutoMapper;
using Meucombustivel.Dtos.Usuario;
using Meucombustivel.Exceptions;
using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;
using Meucombustivel.Services.Interfaces;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.ServicesAuth;
 
namespace Meucombustivel.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IMapper _mapper;
       private readonly ISupabaseAuthService _supabaseAuth;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public UsuarioService(IUsuarioRepository usuarioRepository, IMapper mapper, ISupabaseAuthService supabaseAuth,  IConfiguration configuration, ITokenService tokenService)
        {
            _usuarioRepository = usuarioRepository;
            _mapper = mapper;
            _supabaseAuth = supabaseAuth;
            _tokenService = tokenService;
            _configuration = configuration;
        }

        public async Task<Guid> CreateAsync(CreateUsuarioDto dto)
        {
            var existingUserProfile = await _usuarioRepository.GetByEmailAsync(dto.Email);
            if (existingUserProfile != null)
            {
                throw new InvalidOperationException($"Já existe um perfil de usuário com o e-mail {dto.Email}.");
            }

            try
            {
                var authResponse = await _supabaseAuth.SignUp(dto.Email, dto.Senha);

                if (authResponse?.User == null)
                {
                    throw new InvalidOperationException("Falha ao registrar usuário no Supabase Auth. O e-mail pode já estar em uso ou outro erro ocorreu.");
                }

                var usuarioProfile = new Usuarios();
                _mapper.Map(dto, usuarioProfile);

                usuarioProfile.IdAuth = Guid.Parse(authResponse.User.Id);

                var usuarioCriado = await _usuarioRepository.AddAsync(usuarioProfile);
                return usuarioCriado.Id;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Erro ao registrar usuário: {ex.Message}", ex);
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingUserProfile = await _usuarioRepository.GetByIdAsync(id);
            if (existingUserProfile == null)
            {
                return false;
            }

            await _usuarioRepository.DeleteAsync(id);
            return true;
        }

        public async Task<IEnumerable<ReadUsuarioDto>> GetAllAsync()
        {
            var usuarios = await _usuarioRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ReadUsuarioDto>>(usuarios);
        }

        public async Task<ReadUsuarioDto?> GetByIdAsync(Guid id)
        {
            var usuario = await _usuarioRepository.GetByIdAsync(id);
            if (usuario == null)
            {
                return null;
            }
            return _mapper.Map<ReadUsuarioDto>(usuario);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateUsuarioDto dto)
        {
            var existingUserProfile = await _usuarioRepository.GetByIdAsync(id);
            if (existingUserProfile == null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != existingUserProfile.Email)
            {
                var userWithNewEmail = await _usuarioRepository.GetByEmailAsync(dto.Email);
                if (userWithNewEmail != null)
                {
                    throw new InvalidOperationException($"O e-mail {dto.Email} já está em uso por outro perfil de usuário.");
                }
            }

            _mapper.Map(dto, existingUserProfile);
            await _usuarioRepository.UpdateAsync(existingUserProfile);
            return true;
        }


        public async Task<AuthResponseDto?> AuthenticateAsync(string email, string senha)
        {

            var sessionResponse = await _supabaseAuth.SignIn(email, senha);

            if (sessionResponse?.User == null)
            {
                throw new UnauthorizedException("Credenciais inválidas.");
            }

            var usuarioProfile = await _usuarioRepository.GetByEmailAsync(email);

            if (usuarioProfile == null)
            {
                throw new NotFoundException("Perfil de usuário não encontrado após autenticação.");
            }

            var jwtToken = _tokenService.GenerateJwtToken(usuarioProfile);

            var readUsuarioDto = _mapper.Map<ReadUsuarioDto>(usuarioProfile);
            
           

            return new AuthResponseDto 
            {
                Token = jwtToken,
                User = readUsuarioDto,
                //EmpresaId = usuarioProfile.EmpresaId ?? Guid.Empty

            };

        
        }
    }
}



