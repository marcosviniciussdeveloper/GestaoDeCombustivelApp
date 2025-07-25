using static DashboardDto;

namespace WebApplication1.Services.Interface
{
    public interface IDashBoardService
    {
        Task<List<DashboardMensalDto>> ObterDadosMensaisAsync(Guid? empresaId = null);
    
        Task<DashboardDto> ObterDashboardAsync(Guid? empresaId = null, DateTime? de = null, DateTime? ate = null);
    }
}
 