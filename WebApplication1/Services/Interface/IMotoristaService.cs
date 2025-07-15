using Meucombustivel.Dtos.Motorista;

namespace Meucombustivel.Services.Interfaces
{
    public interface IMotoristaService
    {
        Task<Guid> CreateAsync(Guid usuarioId, CreateMotoristaDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateMotoristaDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<ReadMotoristaDto>> GetAllAsync();
              Task<ReadMotoristaDto?> GetByIdAsync(Guid id);
        Task<Guid> ResgisterNewDriverAsync(RegisterMotoristaDto dto);
        Task<IEnumerable<ReadMotoristaDto>> GetAllByEmpresaAsync(Guid EmpresaId);
    }
}
