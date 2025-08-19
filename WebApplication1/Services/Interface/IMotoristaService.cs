using Meucombustivel.Dtos.Motorista;
using Meucombustivel.Models;
using static Meucombustivel.Dtos.Motorista.ReadMotoristaDto;

public interface IMotoristaService
{
    Task<Guid> CreateAsync(Guid usuarioId,  CreateMotoristaDto dto);
    Task<bool> UpdateAsync(Guid id, UpdateMotoristaDto dto);
    Task<bool> DeleteAsync(Guid id);

    Task<Motorista> UpdateStatusAsync(Guid usuarioId, bool novoStatus);
    Task<IEnumerable<ReadMotoristaDto>> GetAllAsync();
    Task<ReadMotoristaDto?> GetByIdAsync(Guid id);
    Task<Guid> ResgisterNewDriverAsync(RegisterMotoristaDto dto);
    Task<IEnumerable<ReadMotoristaDto>> GetAllByEmpresaAsync(Guid EmpresaId);
    Task<ReadMotoristaDto?> GetByUsuarioIdAsync(Guid usuarioId); 
}
