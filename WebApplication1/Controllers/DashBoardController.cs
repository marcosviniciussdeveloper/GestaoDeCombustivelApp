using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using WebApplication1.Services.Interface;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly IDashBoardService _dashboardService;

        public DashboardController(IDashBoardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDashboard([FromQuery] Guid? empresaId = null, [FromQuery] DateTime? de = null, [FromQuery] DateTime? ate = null)
        {
            var result = await _dashboardService.ObterDashboardAsync(empresaId, de, ate);
            return Ok(result);
        }


        [HttpGet("mensal")]

        public async Task<IActionResult> GetDashboardMensal([FromQuery] Guid? empresaId = null, [FromQuery] DateTime? de = null, [FromQuery] DateTime? ate = null)
        {
            var result = await _dashboardService.ObterDadosMensaisAsync(empresaId);
            return Ok(result);
        }

    }
}