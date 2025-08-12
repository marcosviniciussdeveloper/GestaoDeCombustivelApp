
using WebApplication1.Models.View;
using WebApplication1.Repositories.Interfaces;
using static Supabase.Postgrest.Constants;

public class EmpresaMotoristaService : IEmpresaMotoristaService
{
    private readonly IEmpresaMotoristaRepository _repo;
    private readonly Supabase.Client _supabase;

    public EmpresaMotoristaService(IEmpresaMotoristaRepository repo, Supabase.Client supabase)
    {
        _repo = repo;
        _supabase = supabase;
    }

    public async Task VincularAsync(Guid empresaId, Guid motoristaUsuarioId, string status = "ativo")
    {
        if (await _repo.ExisteVinculoAsync(empresaId, motoristaUsuarioId)) return;
        await _repo.VincularAsync(empresaId, motoristaUsuarioId, status);
    }

    public Task DesvincularAsync(Guid empresaId, Guid motoristaUsuarioId)
        => _repo.DesvincularAsync(empresaId, motoristaUsuarioId);

    public Task AtualizarStatusAsync(Guid empresaId, Guid motoristaUsuarioId, string status)
        => _repo.AtualizarStatusAsync(empresaId, motoristaUsuarioId, status);

    public Task<bool> ExisteVinculoAsync(Guid empresaId, Guid motoristaUsuarioId)
        => _repo.ExisteVinculoAsync(empresaId, motoristaUsuarioId);

    public Task<IReadOnlyList<Guid>> ListarMotoristasIdsPorEmpresaAsync(Guid empresaId)
        => _repo.ListarMotoristasIdsPorEmpresaAsync(empresaId);

    public Task<IReadOnlyList<Guid>> ListarEmpresasIdsPorMotoristaAsync(Guid motoristaUsuarioId)
        => _repo.ListarEmpresasIdsPorMotoristaAsync(motoristaUsuarioId);

    public async Task<IReadOnlyList<VwMotoristaEmpresa>> ListarMotoristasDaEmpresaAsync(Guid empresaId)
    {
        var resp = await _supabase
            .From<VwMotoristaEmpresa>()
            .Where(v => v.EmpresaId = = empresaId)
            .Get();

        return resp.Models;
    }
}
