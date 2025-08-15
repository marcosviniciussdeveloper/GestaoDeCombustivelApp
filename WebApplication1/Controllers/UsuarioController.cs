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
        [Produces("application/json")]
        public async Task<IActionResult> AutenticarUsuario([FromBody] LoginDto login)
        {
            var result = await _usuarioService.AuthenticateAsync(login.Email, login.Senha);
            if (result is null) return Unauthorized(new { error = "Credenciais inválidas." });

            return Ok(new
            {
                token = result.Token,
                user = new
                {
                    id = result.User.Id,
                    nome = result.User.Nome,
                    email = result.User.Email,
                    role = result.User.TipoUsuario,     
                    empresaId = result.User.EmpresaId  
                }
            });
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

    }
}
