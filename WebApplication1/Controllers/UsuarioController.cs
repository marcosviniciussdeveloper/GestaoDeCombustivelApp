using Microsoft.AspNetCore.Mvc;
using Meucombustivel.Dtos.Usuario;
using Meucombustivel.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Meucombustivel.Constants;
using System.Security.Claims;
using Meucombustivel.Exceptions;

namespace Meucombustivel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioService _usuarioService;

        public UsuarioController(IUsuarioService usuarioService)
        {
            _usuarioService = usuarioService;
        }

        [HttpPost("registrar")]
        [AllowAnonymous]
        public async Task<IActionResult> RegistrarUsuario([FromBody] CreateUsuarioDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Guid usuarioId = await _usuarioService.CreateAsync(dto); 
            return CreatedAtAction(nameof(GetById), new { id = usuarioId }, new { id = usuarioId, message = "Usuário registrado com sucesso." });
        }

        [HttpPost("autenticar")]
        [AllowAnonymous]
        public async Task<IActionResult> AutenticarUsuario([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var authResponseDto = await _usuarioService.AuthenticateAsync(loginDto.Email, loginDto.Senha); 
            if (authResponseDto == null)
            {
                
                throw new UnauthorizedException("Credenciais inválidas.");
            }

            return Ok(authResponseDto);
        }

        [HttpGet("{id}")]
        [Authorize (Roles = UserRoles.Administrador + "," + UserRoles.Gestor + "," + UserRoles.Motorista)]
        public async Task<IActionResult> GetById(Guid id)
        {
            var loggedInUserId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var loggedInUserRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (loggedInUserId != id && loggedInUserRole != UserRoles.Administrador)
            {
                throw new UnauthorizedAccessException("Acesso negado: Você só pode ver o seu próprio perfil.");
            }

            var usuario = await _usuarioService.GetByIdAsync(id);
            return Ok(usuario);
        }

        [HttpGet]
        [Authorize(Roles = UserRoles.Administrador)]
        public async Task<IActionResult> GetAll()
        {
            var usuarios = await _usuarioService.GetAllAsync();
            return Ok(usuarios);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Administrador
            )]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUsuarioDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool updated = await _usuarioService.UpdateAsync(id, dto); 
            if (!updated)
            {
               
                throw new NotFoundException($"Usuário com ID {id} não encontrado para atualização.");
            }
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Administrador)]
        public async Task<IActionResult> Delete(Guid id)
        {
            bool deleted = await _usuarioService.DeleteAsync(id); 
            if (!deleted)
            {
              
                throw new NotFoundException($"Usuário com ID {id} não encontrado para exclusão.");
            }
            return NoContent();
        }
    }
}
