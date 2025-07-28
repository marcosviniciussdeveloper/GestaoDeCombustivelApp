using Meucombustivel.Constants;
using Meucombustivel.Dtos.Veiculo;
using Meucombustivel.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meucombustivel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VeiculoController : ControllerBase
    {
        private readonly IVeiculoService _veiculoService;

        public VeiculoController(IVeiculoService veiculoService)
        {
            _veiculoService = veiculoService;
        }

        [HttpPost("registrar")]
        [Authorize(Roles = UserRoles.Administrador + "," + UserRoles.Gestor)]
        public async Task<IActionResult> RegistrarVeiculo([FromBody] CreateVeiculoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guid veiculoId = await _veiculoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = veiculoId }, new { message = "Veículo criado com sucesso!", id = veiculoId });
        }

        [HttpGet("Buscar varios")]

        public async Task<ActionResult<IEnumerable<ReadVeiculoDto>>> GetAllByEmpresaAsync(Guid empresaId)
        {
            var veiculos = await _veiculoService.GetAllByEmpresaAsync(empresaId);
            return Ok(veiculos);
        }

        [HttpGet("Buscar apenas um")]
        [Authorize(Roles = UserRoles.Administrador + "," + UserRoles.Gestor)]
        public async Task<ActionResult<ReadVeiculoDto>> GetById(Guid id)
        {
            var veiculo = await _veiculoService.GetByIdAsync(id);
            if (veiculo == null)
                return NotFound(new { message = "Veículo não encontrado." });

            return Ok(veiculo);
        }

        [HttpPut("atualizar/{id}")]
        [Authorize(Roles = UserRoles.Administrador + "," + UserRoles.Gestor)]
        public async Task<IActionResult> AtualizarVeiculo(Guid id, [FromBody] UpdateVeiculoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _veiculoService.UpdateAsync(id, dto);
            return NoContent();
        }

      
    }
}
