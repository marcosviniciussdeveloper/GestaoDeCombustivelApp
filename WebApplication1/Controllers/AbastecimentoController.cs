using Meucombustivel.Dtos.Abastecimento;
using Meucombustivel.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Meucombustivel.Exceptions;
using Meucombustivel.Constants;

namespace Meucombustivel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AbastecimentoController : ControllerBase
    {
        private readonly IAbastecimentoService _abastecimentoService;

        public AbastecimentoController(IAbastecimentoService abastecimentoService)
        {
            _abastecimentoService = abastecimentoService;
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrador + "," + UserRoles.Gestor + "," + UserRoles.Motorista)]
        public async Task<ActionResult> CreateAbastecimento([FromBody] CreateAbastecimentoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guid abastecimentoId = await _abastecimentoService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = abastecimentoId }, new { message = "Abastecimento registrado com sucesso", id = abastecimentoId });
        }

        [HttpGet("por-veiculo/{veiculoId}")]
        [Authorize(Roles = UserRoles.Administrador + "," + UserRoles.Gestor)]
        public async Task<ActionResult<IEnumerable<ReadAbastecimentoDto>>> GetAllByVeiculo(Guid veiculoId)
        {
            var abastecimentos = await _abastecimentoService.GetAllByVeiculoAsync(veiculoId);
            return Ok(abastecimentos);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ReadAbastecimentoDto>> GetById(Guid id)
        {
            var abastecimento = await _abastecimentoService.GetByIdAsync(id);
            if (abastecimento == null)
                throw new NotFoundException($"Abastecimento com ID {id} não encontrado.");

            return Ok(abastecimento);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = UserRoles.Administrador + "," + UserRoles.Gestor)]
        public async Task<ActionResult> UpdateAsync(Guid id, [FromBody] UpdateAbastecimentoDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _abastecimentoService.UpdateAsync(id, dto);
            if (!updated)
                throw new NotFoundException($"Abastecimento com ID {id} não encontrado para atualização.");

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = UserRoles.Administrador + "," + UserRoles.Gestor)]
        public async Task<ActionResult> DeleteAsync(Guid id)
        {
            var deleted = await _abastecimentoService.DeleteAsync(id);
            if (!deleted)
                throw new NotFoundException($"Abastecimento com ID {id} não encontrado para exclusão.");

            return NoContent();
        }
    
    
    }


}
