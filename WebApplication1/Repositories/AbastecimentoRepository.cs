using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;


namespace Meucombustivel.Repositories
{
    /// <summary>
    /// Implementação da interface IAbastecimentoRepository, utilizando Supabase.Client para acesso aos dados.
    /// Esta classe é responsável por todas as operações de persistência da entidade Abastecimento.
    /// </summary>
    public class AbastecimentoRepository : IAbastecimentoRepository
    {
        private readonly Supabase.Client _supabaseClient;

        public AbastecimentoRepository(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<Abastecimento> AddAsync(Abastecimento entity)
        {
         var response =   await _supabaseClient.From<Abastecimento>().Insert(entity);
            return response.Models.First();
        }


        public async Task DeleteAsync(Guid id)
        {
            await _supabaseClient.From<Abastecimento>()
                                 .Where(a => a.Id == id)
                                 .Delete();
        }
        public async Task<IEnumerable<Abastecimento>> GetAllAsync()
        {
            var response = await _supabaseClient.From<Abastecimento>().Get();
            return response.Models;
        }

        public async Task<Abastecimento?> GetByIdAsync(Guid id)
        {
            var response = await _supabaseClient.From<Abastecimento>()
                                                 .Where(a => a.Id == id)
                                                 .Get();
            return response.Models.FirstOrDefault();
        }

     
        public async Task UpdateAsync(Abastecimento entity)
        {
            await _supabaseClient.From<Abastecimento>()
                                 .Where(a => a.Id == entity.Id)
                                 .Update(entity);
        }


        public async Task<IEnumerable<Abastecimento>> GetAbastecimentosByVeiculoIdAsync(Guid veiculoId)
        {
            var response = await _supabaseClient.From<Abastecimento>()
                                                 .Where(a => a.VeiculoId == veiculoId)
                                                 .Get();
            return response.Models;
        }

        public async Task<IEnumerable<Abastecimento>> GetAbastecimentosByMotoristaIdAsync(Guid motoristaId)
        {
            var response = await _supabaseClient.From<Abastecimento>()
                                                 .Where(a => a.MotoristaId == motoristaId)
                                                 .Get();
            return response.Models;
        }

    
    }
}
