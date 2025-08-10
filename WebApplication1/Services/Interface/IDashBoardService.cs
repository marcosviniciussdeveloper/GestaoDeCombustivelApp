    using static DashboardDto;

    namespace WebApplication1.Services.Interface
    {
        public interface IDashBoardService
        {
            Task<DashboardDto> ObterDashboardAsync(
                Guid? empresaId = null, DateTime? de = null, DateTime? ate = null);

            Task<List<DashboardMensalDto>> ObterDadosMensaisAsync(
                Guid? empresaId = null, DateOnly? de = null, DateOnly? ate = null);
        }
    }
