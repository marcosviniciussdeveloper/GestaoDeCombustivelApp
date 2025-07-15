
using Meucombustivel.Dtos.Usuario;
using System.Numerics;

namespace Meucombustivel.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<Guid> CreateAsync(CreateUsuarioDto dto);
        Task<bool> UpdateAsync(Guid id, UpdateUsuarioDto dto);
        Task<bool> DeleteAsync(Guid id);

        Task<AuthResponseDto?> AuthenticateAsync(string email, string senha);

        Task<ReadUsuarioDto?> GetByIdAsync(Guid id);
       
        Task <IEnumerable <ReadUsuarioDto?>> GetAllAsync();



    }
}
