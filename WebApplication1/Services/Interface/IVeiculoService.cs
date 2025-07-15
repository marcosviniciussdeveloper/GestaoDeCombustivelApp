using Meucombustivel.Dtos.Veiculo;


namespace Meucombustivel.Services.Interfaces
{
    public interface IVeiculoService
    {
        Task<Guid> CreateAsync(CreateVeiculoDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateVeiculoDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<ReadVeiculoDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ReadVeiculoDto>> GetAllByEmpresaAsync(Guid empresaId); 
    }
}
