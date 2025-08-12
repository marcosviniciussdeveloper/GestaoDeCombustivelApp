

using WebApplication1.Models.View;

public interface IEmpresaMotoristaService
{
    Task VincularAsync(Guid empresaId, Guid motoristaUsuarioId, string status = "ativo");
    Task DesvincularAsync(Guid empresaId, Guid motoristaUsuarioId);
    Task AtualizarStatusAsync(Guid empresaId, Guid motoristaUsuarioId, string status);
    Task<bool> ExisteVinculoAsync(Guid empresaId, Guid motoristaUsuarioId);
    Task<IReadOnlyList<Guid>> ListarMotoristasIdsPorEmpresaAsync(Guid empresaId);
    Task<IReadOnlyList<Guid>> ListarEmpresasIdsPorMotoristaAsync(Guid motoristaUsuarioId);
    Task<IReadOnlyList<VwMotoristaEmpresa>> ListarMotoristasDaEmpresaAsync(Guid empresaId);
}
