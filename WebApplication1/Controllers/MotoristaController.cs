using Meucombustivel.Constants;
using Meucombustivel.Dtos.Motorista;
using Meucombustivel.Models;
using Meucombustivel.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Meucombustivel.Dtos.Motorista.ReadMotoristaDto;

namespace Meucombustivel.Controllers
{
    [ApiController]
    [Route("api/motorista")]
    [Produces("application/json")]
    public class MotoristaController : ControllerBase
    {
        private readonly IMotoristaService _motoristaService;
        public MotoristaController(IMotoristaService motoristaService) => _motoristaService = motoristaService;

        [HttpPost("registrar")]
        [Authorize(Roles = UserRoles.Gestor + "," + UserRoles.Administrador)]
        public async Task<IActionResult> RegistrarMotorista([FromQuery] Guid usuarioId, [FromBody] CreateMotoristaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var id = await _motoristaService.CreateAsync(usuarioId,  dto );
            return CreatedAtAction(nameof(GetById), new { id }, new { message = "Motorista criado com sucesso!", id });
        }

        [AllowAnonymous]
        [HttpPost("registrar-sem-empresa")]
        public async Task<IActionResult> RegistrarMotoristaSemEmpresa([FromBody] RegisterMotoristaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var usuarioId = await _motoristaService.ResgisterNewDriverAsync(dto);
            return CreatedAtAction(nameof(GetByUsuarioId), new { usuarioId }, new { message = "Motorista criado com sucesso!", id = usuarioId });
        }

        [HttpPatch("{usuarioId}/status")]
        public async Task<IActionResult> AtualizarStatus(Guid usuarioId, [FromBody] AtualizarStatusDto dto)
        {
            await _motoristaService.UpdateStatusAsync(usuarioId, dto.Status);

         
            return Ok (new{ message =  "Status Atualizado com sucesso"});
        }

        [HttpGet("buscar-varios")]
        public async Task<ActionResult<IEnumerable<ReadMotoristaDto>>> GetAllByEmpresaAsync([FromQuery] Guid empresaId)
        {
            if (empresaId == Guid.Empty)
                return BadRequest(new { error = "O parâmetro empresaId é obrigatório." });

            var dtos = await _motoristaService.GetAllByEmpresaAsync(empresaId);
            return Ok(dtos);
        }


        [HttpGet("buscar/{id:guid}")]
        public async Task<ActionResult<ReadMotoristaDto>> GetById([FromRoute] Guid id)
        {
            var m = await _motoristaService.GetByIdAsync(id);
            if (m == null) return NotFound(new { message = "Motorista não encontrado." });
            return Ok(m);
        }

        [HttpGet("buscar-por-usuario/{usuarioId:guid}")]
        public async Task<ActionResult<ReadMotoristaDto>> GetByUsuarioId([FromRoute] Guid usuarioId)
        {
            var m = await _motoristaService.GetByUsuarioIdAsync(usuarioId);
            if (m == null) return NotFound(new { message = "Motorista não encontrado." });
            return Ok(m);
        }

        [HttpPut("atualizar/{id:guid}")]
        public async Task<IActionResult> UpdateMotorista([FromRoute] Guid id, [FromBody] UpdateMotoristaDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var ok = await _motoristaService.UpdateAsync(id, dto);
            if (!ok) return NotFound(new { message = "Motorista não encontrado." });
            return NoContent();
        }
    }
}
