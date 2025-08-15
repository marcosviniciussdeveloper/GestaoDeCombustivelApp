using Microsoft.AspNetCore.Mvc;


[ApiController]
[Route("api/empresa-motoristas")]
public class EmpresaMotoristasController : ControllerBase
{
    private readonly IEmpresaMotoristaService _empresaMotoristaService;

    public EmpresaMotoristasController(IEmpresaMotoristaService empresaMotoristaService)
    {
        _empresaMotoristaService = empresaMotoristaService;
    }

    [HttpPost("vincular")]
    public async Task<IActionResult> Vincular(Guid empresaId, Guid motoristaUsuarioId, string status = "ativo")
    {
        await _empresaMotoristaService.VincularAsync(empresaId, motoristaUsuarioId, status);
        return Ok();
    }

    [HttpDelete("desvincular")]
    public async Task<IActionResult> Desvincular(Guid empresaId, Guid motoristaUsuarioId)
    {
        await _empresaMotoristaService.DesvincularAsync(empresaId, motoristaUsuarioId);
        return NoContent();
    }

    [HttpPut("atualizar-status")]
    public async Task<IActionResult> AtualizarStatus(Guid empresaId, Guid motoristaUsuarioId, string status)
    {
        await _empresaMotoristaService.AtualizarStatusAsync(empresaId, motoristaUsuarioId, status);
        return Ok();
    }

    
    [HttpGet("existe-vinculo")]
    public async Task<IActionResult> ExisteVinculo(Guid empresaId, Guid motoristaUsuarioId)
    {
        var existe = await _empresaMotoristaService.ExisteVinculoAsync(empresaId, motoristaUsuarioId);
        return Ok(existe);
    }

    [HttpGet("{empresaId}/ids")]
    public async Task<IActionResult> ListarIds(Guid empresaId)
    {
        var ids = await _empresaMotoristaService.ListarMotoristasIdsPorEmpresaAsync(empresaId);
        return Ok(ids);
    }

    [HttpGet("motorista/{motoristaUsuarioId}/empresas")]
    public async Task<IActionResult> ListarEmpresasIds(Guid motoristaUsuarioId)
    {
        var ids = await _empresaMotoristaService.ListarEmpresasIdsPorMotoristaAsync(motoristaUsuarioId);
        return Ok(ids);
    }

    [HttpGet("{empresaId}/lista")]
    public async Task<IActionResult> ListarMotoristasDaEmpresa(Guid empresaId)
    {
        var motoristas = await _empresaMotoristaService.ListarMotoristasDaEmpresaAsync(empresaId);
        return Ok(motoristas);
    }
}