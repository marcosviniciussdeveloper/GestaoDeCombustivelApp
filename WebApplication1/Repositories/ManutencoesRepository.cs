using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;


namespace Meucombustivel.Repositories
{
    /// <summary>
    /// Implementação da interface IManutencaoRepository, utilizando Supabase.Client para acesso aos dados.
    /// Esta classe é responsável por todas as operações de persistência da entidade Manutencao.
    /// </summary>
    public class ManutencoesRepository : IManutencaoRepository
    {
        private readonly Supabase.Client _supabaseClient;

      
        public ManutencoesRepository(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }


        public async Task<Manutencoes> AddAsync(Manutencoes entity)
        {
         var response =    await _supabaseClient.From<Manutencoes>().Insert(entity);
            return response.Models.First();
        }

        public async Task DeleteAsync(Guid id)
        {
            await _supabaseClient.From<Manutencoes>()
                                 .Where(m => m.Id == id)
                                 .Delete();
        }

        public async Task<IEnumerable<Manutencoes>> GetAllAsync()
        {
            var response = await _supabaseClient.From<Manutencoes>().Get();
            return response.Models;
        }

        public async Task<Manutencoes?> GetByIdAsync(Guid id)
        {
            var response = await _supabaseClient.From<Manutencoes>()
                                                 .Where(m => m.Id == id)
                                                 .Get();
            return response.Models.FirstOrDefault();
        }

        public async Task UpdateAsync(Manutencoes entity)
        {
            await _supabaseClient.From<Manutencoes>()
                                 .Where(m => m.Id == entity.Id)
                                 .Update(entity);
        }

        public async Task<IEnumerable<Manutencoes>> GetManutencoesByVeiculoIdAsync(Guid veiculoId)
        {
            var response = await _supabaseClient.From<Manutencoes>()
                                                 .Where(m => m.IdVeiculo == veiculoId)
                                                 .Get();
            return response.Models;
        }
    }
}
