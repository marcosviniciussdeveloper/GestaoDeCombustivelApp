using Meucombustivel.Constants;
using Meucombustivel.Dtos.Motorista;
using Meucombustivel.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meucombustivel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class MotoristaController : ControllerBase
    {
        private readonly IMotoristaService _motoristaService;

        public MotoristaController(IMotoristaService motoristaService)
        {
            _motoristaService = motoristaService;
        }

        [HttpPost("registrar")]
        [Authorize(Roles = UserRoles.Gestor + "," +UserRoles.Administrador  )]
        public async Task<IActionResult> RegistrarMotorista(Guid usuarioId, [FromBody] CreateMotoristaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guid motoristaId = await _motoristaService.CreateAsync(usuarioId, dto);
            return CreatedAtAction(nameof(GetById), new { id = motoristaId }, new { message = "Motorista criado com sucesso!", id = motoristaId });
        }

        [AllowAnonymous] 
        [HttpPost("Registrar Sem empresa")]
        public async Task<IActionResult> RegistrarMotoristaSemEmpresa([FromBody] RegisterMotoristaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guid motoristaId = await _motoristaService.ResgisterNewDriverAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = motoristaId }, new { message = "Motorista criado com sucesso!", id = motoristaId });
        }

        [HttpGet("Buscar varios")]
        [Authorize(Roles = UserRoles.Administrador + "," + UserRoles.Gestor)]
        public async Task<ActionResult<IEnumerable<ReadMotoristaDto>>> GetAllByEmpresaAsync(Guid empresaId)
        {
            var motoristas = await _motoristaService.GetAllByEmpresaAsync(empresaId);
            return Ok(motoristas);
        }

        [HttpGet("Buscar apenas um")]
        public async Task<ActionResult<ReadMotoristaDto>> GetById(Guid id)
        {
            var motorista = await _motoristaService.GetByIdAsync(id);
            if (motorista == null)
                return NotFound(new { message = "Motorista não encontrado." });

            return Ok(motorista);
        }

        [HttpPut("atualizar/{id}")]
        public async Task<IActionResult> UpdateMotorista(Guid id, [FromBody] UpdateMotoristaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _motoristaService.UpdateAsync(id, dto);
            if (!updated)
                return NotFound(new { message = "Motorista não encontrado." });

            return NoContent();
        }

        [HttpDelete("deletar/{id}")]
        [Authorize(Roles = UserRoles.Administrador)]
        public async Task<IActionResult> DeleteMotorista(Guid id)
        {
            var deleted = await _motoristaService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "Motorista não encontrado." });

            return NoContent();
        }
    }
}


