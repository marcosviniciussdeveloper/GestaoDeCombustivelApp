using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;


namespace Meucombustivel.Repositories
{
    public class MotoristaRepository : IMotoristaRepository
    {
        private readonly Supabase.Client _supabaseClient;

        public MotoristaRepository(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<Motorista> AddAsync(Motorista entity)
        {
            var response = await _supabaseClient.From<Motorista>().Insert(entity);

           
            if (response.ResponseMessage.IsSuccessStatusCode && response.Models.Any())
            {
                return response.Models.First();
            }
            else
            {
                string errorMessage = $"Falha ao inserir motorista no banco de dados. Status: {response.ResponseMessage.StatusCode}. ";
                if (response.ResponseMessage.Content != null)
                {
                    errorMessage += $"Detalhes: {await response.ResponseMessage.Content.ReadAsStringAsync()}";
                }
              
                if (response.ResponseMessage.IsSuccessStatusCode && !response.Models.Any())
                {
                    errorMessage = "Erro de inserção: Supabase indicou sucesso, mas nenhum registro foi retornado. Verifique constraints ou RLS.";
                }
                throw new InvalidOperationException(errorMessage);
            }
        }

        public async Task DeleteAsync(Guid id)
        {
            await _supabaseClient.From<Motorista>()
                                 .Where(m => m.UsuarioId == id)
                                 .Delete();
        }

        public async Task<IEnumerable<Motorista>> GetAllAsync()
        {
            var response = await _supabaseClient.From<Motorista>().Get();
            return response.Models;
        }

        public async Task<Motorista?> GetByIdAsync(Guid id)
        {
            var response = await _supabaseClient.From<Motorista>()
                                                 .Where(m => m.UsuarioId == id)
                                                 .Get();    
            return response.Models.FirstOrDefault();
        }

        public async Task UpdateAsync(Motorista entity)
        {
            await _supabaseClient.From<Motorista>()
                                 .Where(m => m.UsuarioId == entity.UsuarioId)
                                 .Update(entity);
        }

        public async Task<Motorista?> GetByUsuarioIdAsync(Guid usuarioId)
        {
            var response = await _supabaseClient.From<Motorista>()
                                                 .Where(m => m.UsuarioId == usuarioId)
                                                 .Get();
            return response.Models.FirstOrDefault();
        }
    }
}
