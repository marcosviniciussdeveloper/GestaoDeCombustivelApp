using Meucombustivel.Dtos.Motorista;
using WebApplication1.Models;

public interface IEmpresaMotoristaService
{
    Task VincularAsync(Guid empresaId, Guid motoristaUsuarioId, string status = "ativo");
    Task DesvincularAsync(Guid empresaId, Guid motoristaUsuarioId);
    Task AtualizarStatusAsync(Guid empresaId, Guid motoristaUsuarioId, string status);
    Task<bool> ExisteVinculoAsync(Guid empresaId, Guid motoristaUsuarioId);
    Task<IReadOnlyList<Guid>> ListarMotoristasIdsPorEmpresaAsync(Guid empresaId);
    Task<IReadOnlyList<Guid>> ListarEmpresasIdsPorMotoristaAsync(Guid motoristaUsuarioId);
    Task<EmpresaMotorista?> ObterAsync(Guid empresaId, Guid motoristaUsuarioId);

    Task<IReadOnlyList<ReadMotoristaDto>> ListarMotoristasDaEmpresaAsync(Guid empresaId);
}
