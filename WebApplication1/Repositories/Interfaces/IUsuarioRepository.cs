using Meucombustivel.Models;
namespace Meucombustivel.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de acesso a dados da entidade Usuario.
    /// Herda de IGenericRepository para operações CRUD básicas.
    /// </summary>
    public interface IUsuarioRepository : IGenericRepository<Usuarios>
    {
        Task <Usuarios?> GetByCpfAsync(string cpf);

        /// <summary>
        /// Busca um usuário pelo seu endereço de e-mail.
        /// </summary>
        /// <param name="email">O endereço de e-mail do usuário.</param>
        /// <returns>O objeto Usuarios encontrado ou null se não existir.</returns>
        Task<Usuarios?> GetByEmailAsync(string email);

        
    }
}