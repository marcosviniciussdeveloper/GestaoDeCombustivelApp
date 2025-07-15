using Meucombustivel.Dtos.Manutencao;


namespace Meucombustivel.Services.Interfaces
{
    public interface IManutencaoService
    {
        Task<Guid> CreateAsync(CreateManutencaoDto dto);
        Task<bool> UpdateAsync(Guid id , UpdateManutencaoDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<ReadManutencaoDto?> GetByIdAsync(Guid id);
        Task<IEnumerable<ReadManutencaoDto>> GetAllByVeiculoAsync(Guid id_veiculo);
    }
}
