using Meucombustivel.Dtos.Empresa;


namespace Meucombustivel.Services.Interfaces
{
    public interface IEmpresaService
    {
        Task<Guid> CreateAsync(CreateEmpresaDto dto);
        Task<ReadEmpresaDto?> GetByIdAsync(Guid id);
        Task<bool> UpdateAsync(Guid id, UpdateEmpresaDto dto);
        Task<bool> DeleteAsync(Guid id);
       Task<ReadEmpresaDto> RegisterCompanyAndAdminAsync(RegisterCompanyAndAdminDto dto);
    }
}
