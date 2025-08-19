using Meucombustivel.Dtos.Motorista;
using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;
using WebApplication1.Models.View;


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
            var resp = await _supabaseClient
       .From<Motorista>()
       .Where(m => m.UsuarioId == id)   
       .Get();

            return resp.Models.FirstOrDefault();
        }

        public async Task UpdateAsync(Motorista entity)
        {
            await _supabaseClient.From<Motorista>()
                                 .Where(m => m.UsuarioId == entity.UsuarioId)
                                 .Update(entity);
        }


        public async Task<IReadOnlyList<ReadMotoristaDto>> ListarPorEmpresaAsync(Guid empresaId)
        {
            var resp = await _supabaseClient
                .From<VwMotoristaEmpresa>()
                .Where(v => v.EmpresaId == empresaId)

                .Order(v => v.Nome!, Supabase.Postgrest.Constants.Ordering.Ascending)
                .Get();

            return resp.Models.Select(v => new ReadMotoristaDto
            {

                MotoristaId = v.MotoristaId,
                Nome = v.Nome,
                NumeroCnh = v.NumeroCnh,
                CategoriaCnh = v.CategoriaCnh,
                Datetime = v.ValidadeCnh




            }            ).ToList();
        }
        


        public async Task<Motorista> StatusMotoristaAsync(Guid usuarioId, bool novoStatus)
        {
            var resp = await _supabaseClient
                .From<Motorista>()
                .Where(m => m.UsuarioId == usuarioId)   
                .Set(m => m.Status, novoStatus)
                .Update();

            var atualizado = resp.Models.FirstOrDefault();
            if (atualizado is null)
                throw new KeyNotFoundException($"Motorista com usuario_id {usuarioId} não encontrado ou sem permissão para atualizar.");

            atualizado.Status = novoStatus;

            return atualizado;
        }
    }
}