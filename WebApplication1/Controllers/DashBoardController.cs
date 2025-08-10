using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services.Interface;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly IDashBoardService _svc;
    public DashboardController(IDashBoardService svc) => _svc = svc;

    private static (DateOnly De, DateOnly Ate) Range(DateOnly? de, DateOnly? ate)
    {
        var hoje = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var d1 = de  ?? hoje.AddDays(-30);
        var d2 = ate ?? hoje;
        if (d1 > d2) (d1, d2) = (d2, d1);
        return (d1, d2);
    }

    [HttpGet]
[HttpGet]
public async Task<IActionResult> Get([FromQuery] Guid? empresaId, [FromQuery] DateOnly? de, [FromQuery] DateOnly? ate)
{
    var hoje = DateOnly.FromDateTime(DateTime.UtcNow.Date);
    var d1 = de  ?? hoje.AddDays(-30);
    var d2 = ate ?? hoje;
    if (d1 > d2) (d1, d2) = (d2, d1);

    var deDt  = d1.ToDateTime(TimeOnly.MinValue);
    var ateDt = d2.ToDateTime(TimeOnly.MaxValue);

    var result = await _svc.ObterDashboardAsync(empresaId, deDt, ateDt);
    return Ok(result);
}

[HttpGet("mensal")]
public async Task<IActionResult> GetMensal([FromQuery] Guid? empresaId, [FromQuery] DateOnly? de, [FromQuery] DateOnly? ate)
{
    var hoje = DateOnly.FromDateTime(DateTime.UtcNow.Date);
    var d1 = de  ?? new DateOnly(hoje.Year, hoje.Month, 1).AddMonths(-5);
    var d2 = ate ?? new DateOnly(hoje.Year, hoje.Month, 1);
    if (d1 > d2) (d1, d2) = (d2, d1);

    var result = await _svc.ObterDadosMensaisAsync(empresaId, d1, d2);
    return Ok(result);
}

}