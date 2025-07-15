using Meucombustivel.Dtos.Manutencao;
using Meucombustivel.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;



namespace Meucombustivel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManutencaoController : ControllerBase
    {
        private readonly IManutencaoService _manutencaoService;

        public ManutencaoController(IManutencaoService manutencaoService)
        {
            _manutencaoService = manutencaoService;
        }

        [HttpPost("registrar")]
        public async Task<IActionResult> RegistrarManutencao([FromBody] CreateManutencaoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                Guid manutencaoId = await _manutencaoService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = manutencaoId }, new { message = "Manutenção registrada com sucesso" ,id = manutencaoId });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro interno ao registrar a manutenção.", error = ex.Message });
            }
        }

        [HttpGet("Buscar varios")]
        public async Task<ActionResult<IEnumerable<ReadManutencaoDto>>> GetAllByVeiculoAsync(Guid veiculoId)
        {
            try
            {
                var manutencoes = await _manutencaoService.GetAllByVeiculoAsync(veiculoId);
                return Ok(manutencoes);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro ao buscar as manutenções.", error = ex.Message });
            }
        }

        [HttpGet("Buscar apenas um")]
        public async Task<ActionResult<ReadManutencaoDto>> GetById(Guid id)
        {
            try
            {
                var manutencao = await _manutencaoService.GetByIdAsync(id);
                if (manutencao == null)
                {
                    return NotFound(new { message = "Manutenção não encontrada." });
                }
                return Ok(manutencao);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro ao buscar a manutenção.", error = ex.Message });
            }
        }

        [HttpPut("atualizar/{id}")]
        public async Task<IActionResult> UpdateManutencao(Guid id, [FromBody] UpdateManutencaoDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updated = await _manutencaoService.UpdateAsync(id, dto);
                if (!updated)
                {
                    return NotFound(new { message = "Manutenção não encontrada." });
                }
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Ocorreu um erro ao atualizar a manutenção.", error = ex.Message });
            }
        }
    }

}
