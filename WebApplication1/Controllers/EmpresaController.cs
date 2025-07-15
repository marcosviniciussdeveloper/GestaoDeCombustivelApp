using Meucombustivel.Dtos.Empresa;
using Meucombustivel.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Meucombustivel.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EmpresaController : ControllerBase
    {
        private readonly IEmpresaService _empresaService;

        public EmpresaController(IEmpresaService empresaService)
        {
            _empresaService = empresaService;
        }

        [HttpPost("registrar")]
        [AllowAnonymous] 
        public async Task<IActionResult> RegistrarEmpresa([FromBody] CreateEmpresaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Guid empresaId = await _empresaService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = empresaId }, new { id = empresaId });
        }

        [HttpPost("registrar-com-admin")]
        [AllowAnonymous] 
        public async Task<IActionResult> RegistrarEmpresaEAdmin([FromBody] RegisterCompanyAndAdminDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var empresaCriada = await _empresaService.RegisterCompanyAndAdminAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = empresaCriada.Id }, empresaCriada);
        }

        [HttpGet("buscar")]
        public async Task<ActionResult<ReadEmpresaDto>> GetById(Guid id)
        {
            var empresa = await _empresaService.GetByIdAsync(id);
            if (empresa == null)
                return NotFound(new { message = "Empresa não encontrada." });

            return Ok(empresa);
        }

        [HttpPut("atualizar")]
        public async Task<IActionResult> AtualizarEmpresa(Guid id, [FromBody] UpdateEmpresaDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updated = await _empresaService.UpdateAsync(id, dto);
            if (!updated)
                return NotFound(new { message = "Empresa não encontrada." });

            return Ok(new { message = "Empresa atualizada com sucesso." });
        }

        [HttpDelete("deletar")]
        public async Task<IActionResult> DeletarEmpresa(Guid id)
        {
            var deleted = await _empresaService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "Empresa não encontrada." });

            return NoContent();
        }
    }
}
