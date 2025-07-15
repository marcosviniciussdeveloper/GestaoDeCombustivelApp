using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;


namespace Meucombustivel.Repositories
{
    /// <summary>
    /// Implementação da interface IVeiculoRepository, utilizando Supabase.Client para acesso aos dados.
    /// Esta classe é responsável por todas as operações de persistência da entidade Veiculo.
    /// </summary>
    public class VeiculoRepository : IVeiculoRepository
    {
        private readonly Supabase.Client _supabaseClient;

    
        public VeiculoRepository(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }

        public async Task<Veiculo> AddAsync(Veiculo entity)
        {
           var response =  await _supabaseClient.From<Veiculo>().Insert(entity);
            return response.Models.First();

        }


        public async Task DeleteAsync(Guid id)
        {
            await _supabaseClient.From<Veiculo>()
                                 .Where(v => v.Id == id)
                                 .Delete();
        }

      
        public async Task<IEnumerable<Veiculo>> GetAllAsync()
        {
            var response = await _supabaseClient.From<Veiculo>().Get();
            return response.Models;
        }

      
        public async Task<Veiculo?> GetByIdAsync(Guid id)
        {
            var response = await _supabaseClient.From<Veiculo>()
                                                 .Where(v => v.Id == id)
                                                 .Get();
            return response.Models.FirstOrDefault();
        }

        public async Task UpdateAsync(Veiculo entity)
        {
            await _supabaseClient.From<Veiculo>()
                                 .Where(v => v.Id == entity.Id)
                                 .Update(entity);
        }

        
        public async Task<Veiculo?> GetByPlacaAsync(string placa)
        {
            var response = await _supabaseClient.From<Veiculo>()
                                                 .Where(v => v.Placa == placa)
                                                 .Get();
            return response.Models.FirstOrDefault();
        }

        public async Task<IEnumerable<Veiculo>> GetVeiculosByEmpresaIdAsync(Guid empresaId)
        {
            var response = await _supabaseClient.From<Veiculo>()
                                                 .Where(v => v.EmpresaId == empresaId)
                                                 .Get();
            return response.Models;
        }
    }
}
