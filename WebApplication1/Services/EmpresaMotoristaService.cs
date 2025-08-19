using Meucombustivel.Dtos.Motorista;
using Meucombustivel.Repositories;
using Meucombustivel.Repositories.Interfaces;
using WebApplication1.Models;
using WebApplication1.Models.View;
using WebApplication1.Repositories.Interfaces;
using static Supabase.Postgrest.Constants;

public class EmpresaMotoristaService : IEmpresaMotoristaService
{
    private readonly IEmpresaMotoristaRepository _repo;
    private readonly Supabase.Client _supabase;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IMotoristaRepository _motoristaRepository;

    public EmpresaMotoristaService(
        IUsuarioRepository usuarioRepository,
        IMotoristaRepository motoristaRepository,
        IEmpresaMotoristaRepository repo,
        Supabase.Client supabase)
    {
        _repo = repo;
        _motoristaRepository = motoristaRepository;
        _usuarioRepository = usuarioRepository;
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

    public Task<EmpresaMotorista?> ObterAsync(Guid empresaId, Guid motoristaUsuarioId)
        => _repo.ObterAsync(empresaId, motoristaUsuarioId);

    public async Task<IReadOnlyList<ReadMotoristaDto>> ListarMotoristasDaEmpresaAsync(Guid empresaId)
    {
        var vinc = await _supabase
            .From<EmpresaMotorista>()
            .Filter("empresa_id", Operator.Equals, empresaId.ToString())
            .Get();

        var dtos = new List<ReadMotoristaDto>();

        foreach (var v in vinc.Models)
        {
            if (v == null || v.MotoristaUsuarioId == Guid.Empty) continue;

            var usuario = await _usuarioRepository.GetByIdAsync(v.MotoristaUsuarioId);
            var motorista = await _motoristaRepository.GetByIdAsync(v.MotoristaUsuarioId);
            if (usuario == null || motorista == null) continue;

            dtos.Add(new ReadMotoristaDto
            {
                MotoristaId = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                Cpf = usuario.Cpf,
                NumeroCnh = motorista.NumeroCnh,
                Datetime = motorista.ValidadeCnh,
                CategoriaCnh = motorista.CategoriaCnh
            });
        }

        return dtos;
    }

   
}
