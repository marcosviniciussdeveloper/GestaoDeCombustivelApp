using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;

namespace Meucombustivel.Repositories
{
    /// <summary>
    /// Implementação da interface IUsuarioRepository, utilizando Supabase.Client para acesso aos dados.
    /// Esta classe é responsável por todas as operações de persistência da entidade Usuario.
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly Supabase.Client _supabaseClient;

        public UsuarioRepository(Supabase.Client supabaseClient) 
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<Usuarios> AddAsync(Usuarios entity)
        {
            var response = await _supabaseClient.From<Usuarios>()
                .Insert(entity, new Supabase.Postgrest.QueryOptions
                {
                    Returning = Supabase.Postgrest.QueryOptions.ReturnType.Representation
                });

            return response.Models.First(); 
        }

        public async Task DeleteAsync(Guid id)
        {
            await _supabaseClient.From<Usuarios>()
                                 .Where(u => u.Id == id)
                                 .Delete();
        }

        public async Task<IEnumerable<Usuarios>> GetAllAsync()
        {
            var response = await _supabaseClient.From<Usuarios>().Get();
            return response.Models;
        }

        public async Task<Usuarios?> GetByIdAsync(Guid id)
        {
            var response = await _supabaseClient.From<Usuarios>()
                                                 .Where(u => u.Id == id)
                                                 .Get();
            return response.Models.FirstOrDefault();
        }

        public async Task UpdateAsync(Usuarios entity)
        {
            await _supabaseClient.From<Usuarios>()
                                 .Where(u => u.Id == entity.Id)
                                 .Update(entity);
        }

        public async Task<Usuarios?> GetByEmailAsync(string email)
        {
            var response = await _supabaseClient.From<Usuarios>()
                                                 .Where(u => u.Email == email)
                                                 .Get();
            return response.Models.FirstOrDefault();
        }

        public async Task<Usuarios?> GetByCpfAsync(string cpf)
        {
            var response = await _supabaseClient.From<Usuarios>()
                                                 .Where(u => u.Cpf == cpf)
                                                 .Get();
            if (response.Models.Count == 0)
            {
                return null;
            }

            if (response.Models.Count > 1)
            {
                throw new InvalidOperationException($"Mais de um usuário encontrado com o CPF {cpf}. Verifique a integridade dos dados.");
            }

            return response.Models.First();
        }

    }
}
