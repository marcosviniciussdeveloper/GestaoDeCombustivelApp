using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/empresa-motoristas")]
public class EmpresaMotoristasController : ControllerBase
{
    private readonly IEmpresaMotoristaService _svc;
    public EmpresaMotoristasController(IEmpresaMotoristaService svc) => _svc = svc;

    [HttpPost("{empresaId:guid}/{motoristaUsuarioId:guid}")]
    public async Task<IActionResult> Vincular(Guid empresaId, Guid motoristaUsuarioId)
    {
        await _svc.VincularAsync(empresaId, motoristaUsuarioId);
        return NoContent();
    }

    [HttpDelete("{empresaId:guid}/{motoristaUsuarioId:guid}")]
    public async Task<IActionResult> Desvincular(Guid empresaId, Guid motoristaUsuarioId)
    {
        await _svc.DesvincularAsync(empresaId, motoristaUsuarioId);
        return NoContent();
    }

    [HttpGet("{empresaId:guid}/lista")]
    public async Task<IActionResult> Listar(Guid empresaId)
    {
        var lista = await _svc.ListarMotoristasDaEmpresaAsync(empresaId);
        return Ok(lista);
    }
}
