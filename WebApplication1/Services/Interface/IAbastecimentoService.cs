using Meucombustivel.Dtos.Abastecimento;



namespace Meucombustivel.Services.Interfaces
{
    public interface IAbastecimentoService
    {
        Task<Guid> CreateAsync(CreateAbastecimentoDto dto);
        Task<bool> UpdateAsync(Guid id , UpdateAbastecimentoDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<ReadAbastecimentoDto?> GetByIdAsync( Guid id);

        Task<IEnumerable <ReadAbastecimentoDto>> GetAllByVeiculoAsync (Guid veiculoId);
    }
}
