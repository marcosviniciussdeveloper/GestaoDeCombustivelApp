using AutoMapper;
using Meucombustivel.Dtos.Usuario;
using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;
using Meucombustivel.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Supabase.Gotrue;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.ServicesAuth;


namespace MeuCombustivelTeste.Services
{
    public class UsuarioServiceTests
    {
        private readonly Mock<IUsuarioRepository> _mockUsuarioRepository;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<ISupabaseAuthService> _mockSupabaseAuthService;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<ITokenService> _mockTokenService;

        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            _mockUsuarioRepository = new Mock<IUsuarioRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockSupabaseAuthService = new Mock<ISupabaseAuthService>();
            _mockConfiguration = new Mock<IConfiguration>();
            _mockTokenService = new Mock<ITokenService>();

            _usuarioService = new UsuarioService(
                _mockUsuarioRepository.Object,
                _mockMapper.Object,
                _mockSupabaseAuthService.Object,
                _mockConfiguration.Object,
                _mockTokenService.Object
            );
        }

        [Fact]
        public async Task CreateAsync_DeveRetornarGuidQuandoUsuarioCriadoComSucesso()
        {
            var createDto = new CreateUsuarioDto
            {
                Nome = "Novo Usuário",
                Email = "novo@email.com",
                Senha = "SenhaSegura123",
            };

            var idAuth = Guid.NewGuid().ToString();
            var usuarioMapeado = new Usuarios
            {
                Nome = createDto.Nome,
                Email = createDto.Email,
                Cpf = "12345678900",
                TipoUsuario = "Padrao"
            };

            _mockUsuarioRepository.Setup(repo => repo.GetByEmailAsync(createDto.Email)).ReturnsAsync((Usuarios)null);
            _mockSupabaseAuthService.Setup(s => s.SignUp(createDto.Email, createDto.Senha))
                                    .ReturnsAsync(new Session { User = new User { Id = idAuth } });

            _mockMapper.Setup(m => m.Map<Usuarios>(createDto)).Returns(usuarioMapeado);

            _mockUsuarioRepository.Setup(repo => repo.AddAsync(It.IsAny<Usuarios>()))
                                  .ReturnsAsync((Usuarios u) =>
                                  {
                                      u.Id = Guid.NewGuid();
                                      return u;
                                  });

            _mockTokenService.Setup(s => s.GenerateJwtToken(It.IsAny<Usuarios>()))
                             .Returns("jwt_token_fake");

            var result = await _usuarioService.CreateAsync(createDto);

            Assert.NotEqual(Guid.Empty, result);
            _mockSupabaseAuthService.Verify(s => s.SignUp(createDto.Email, createDto.Senha), Times.Once);
            _mockUsuarioRepository.Verify(repo => repo.AddAsync(It.IsAny<Usuarios>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_DeveLancarExcecaoQuandoEmailJaExiste()
        {
            var createDto = new CreateUsuarioDto
            {
                Nome = "Usuário Existente",
                Email = "existente@email.com",
                Senha = "SenhaSegura123",
            };

            _mockUsuarioRepository.Setup(repo => repo.GetByEmailAsync(createDto.Email))
                                  .ReturnsAsync(new Usuarios());

            await Assert.ThrowsAsync<InvalidOperationException>(() => _usuarioService.CreateAsync(createDto));

            _mockSupabaseAuthService.Verify(s => s.SignUp(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
            _mockUsuarioRepository.Verify(repo => repo.AddAsync(It.IsAny<Usuarios>()), Times.Never);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarDtoQuandoUsuarioEncontrado()
        {
            var usuarioId = Guid.NewGuid();
            var usuario = new Usuarios { Id = usuarioId, Nome = "Teste", Email = "teste@email.com" };
            var dto = new ReadUsuarioDto { Id = usuarioId, Nome = "Teste", Email = "teste@email.com" };

            _mockUsuarioRepository.Setup(repo => repo.GetByIdAsync(usuarioId)).ReturnsAsync(usuario);
            _mockMapper.Setup(m => m.Map<ReadUsuarioDto>(usuario)).Returns(dto);

            var result = await _usuarioService.GetByIdAsync(usuarioId);

            Assert.NotNull(result);
            Assert.Equal(dto.Id, result.Id);
            Assert.Equal(dto.Email, result.Email);
        }

        [Fact]
        public async Task GetByIdAsync_DeveRetornarNullQuandoUsuarioNaoEncontrado()
        {
            var usuarioId = Guid.NewGuid();
            _mockUsuarioRepository.Setup(repo => repo.GetByIdAsync(usuarioId)).ReturnsAsync((Usuarios)null);

            var result = await _usuarioService.GetByIdAsync(usuarioId);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarTrueQuandoUsuarioAtualizado()
        {
            var usuarioId = Guid.NewGuid();
            var usuario = new Usuarios { Id = usuarioId, Nome = "Antigo", Email = "antigo@email.com" };
            var dto = new UpdateUsuarioDto { Nome = "Novo", Email = "novo@email.com" };

            _mockUsuarioRepository.Setup(repo => repo.GetByIdAsync(usuarioId)).ReturnsAsync(usuario);
            _mockUsuarioRepository.Setup(repo => repo.GetByEmailAsync(dto.Email)).ReturnsAsync((Usuarios)null);
            _mockUsuarioRepository.Setup(repo => repo.UpdateAsync(It.IsAny<Usuarios>())).Returns(Task.CompletedTask);

            _mockMapper.Setup(m => m.Map(dto, usuario));

            var result = await _usuarioService.UpdateAsync(usuarioId, dto);

            Assert.True(result);
            _mockUsuarioRepository.Verify(repo => repo.UpdateAsync(usuario), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_DeveRetornarFalseQuandoUsuarioNaoExiste()
        {
            var usuarioId = Guid.NewGuid();
            var dto = new UpdateUsuarioDto { Nome = "Nome" };

            _mockUsuarioRepository.Setup(repo => repo.GetByIdAsync(usuarioId)).ReturnsAsync((Usuarios)null);

            var result = await _usuarioService.UpdateAsync(usuarioId, dto);

            Assert.False(result);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarTrueQuandoUsuarioDeletado()
        {
            var usuarioId = Guid.NewGuid();
            _mockUsuarioRepository.Setup(repo => repo.GetByIdAsync(usuarioId)).ReturnsAsync(new Usuarios { Id = usuarioId });
            _mockUsuarioRepository.Setup(repo => repo.DeleteAsync(usuarioId)).Returns(Task.CompletedTask);

            var result = await _usuarioService.DeleteAsync(usuarioId);

            Assert.True(result);
        }

        [Fact]
        public async Task DeleteAsync_DeveRetornarFalseQuandoUsuarioNaoEncontrado()
        {
            var usuarioId = Guid.NewGuid();
            _mockUsuarioRepository.Setup(repo => repo.GetByIdAsync(usuarioId)).ReturnsAsync((Usuarios)null);

            var result = await _usuarioService.DeleteAsync(usuarioId);

            Assert.False(result);
        }

        [Fact]
        public async Task AuthenticateAsync_DeveRetornarAuthResponseDtoQuandoCredenciaisValidas()
        {
            var email = "teste@email.com";
            var senha = "senha123";
            var usuario = new Usuarios
            {
                Id = Guid.NewGuid(),
                Nome = "Usuário",
                Email = email,
                TipoUsuario = "Padrao"
            };

            _mockSupabaseAuthService.Setup(s => s.SignIn(email, senha))
                .ReturnsAsync(new Session
                {
                    User = new User { Id = Guid.NewGuid().ToString() },
                    RefreshToken = "refresh_token_fake"
                });

            _mockUsuarioRepository.Setup(r => r.GetByEmailAsync(email)).ReturnsAsync(usuario);

            _mockTokenService.Setup(t => t.GenerateJwtToken(usuario)).Returns("jwt_token_fake");

            var dto = new ReadUsuarioDto
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email
            };

            _mockMapper.Setup(m => m.Map<ReadUsuarioDto>(usuario)).Returns(dto);

            var result = await _usuarioService.AuthenticateAsync(email, senha);

            Assert.NotNull(result);
            Assert.Equal("jwt_token_fake", result.AccessToken);
            Assert.Equal("refresh_token_fake", result.RefreshToken);
            Assert.Equal(email, result.Usuario.Email);
        }
    }
}
