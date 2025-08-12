using Meucombustivel.Models;
using Meucombustivel.Repositories.Interfaces;
namespace Meucombustivel.Repositories
{
    /// <summary>
    /// Implementação da interface IEmpresaRepository, utilizando Supabase.Client para acesso aos dados.
    /// Esta classe é responsável por todas as operações de persistência da entidade Empresa.
    /// </summary>
    public class EmpresaRepository : IEmpresaRepository
    {
        private readonly Supabase.Client _supabaseClient;

       
        public EmpresaRepository(Supabase.Client supabaseClient)
        {
            _supabaseClient = supabaseClient;
        }


        public async Task<Empresa> AddAsync(Empresa entity)
        {
           var response =  await _supabaseClient.From<Empresa>().Insert(entity);

            return response.Models.First();
        }


        public async Task DeleteAsync(Guid id)
        {
            await _supabaseClient.From<Empresa>()
                                 .Where(e => e.Id == id)
                                 .Delete();
        }


        public async Task<IEnumerable<Empresa>> GetAllAsync()
        {
            var response = await _supabaseClient.From<Empresa>().Get();
            return response.Models;
        }

        public async Task<Empresa?> GetByIdAsync(Guid id)
        {
            var response = await _supabaseClient.From<Empresa>()
                                                 .Where(e => e.Id == id)
                                                 .Get();
            return response.Models.FirstOrDefault();
        }

  
        public async Task UpdateAsync(Empresa entity)
        {
            await _supabaseClient.From<Empresa>()
                                 .Where(e => e.Id == entity.Id)
                                 .Update(entity);
        }

        public async Task<Empresa?> GetByCnpjAsync(string cnpj)
        {
            var response = await _supabaseClient.From<Empresa>()
                                                 .Where(e => e.Cnpj == cnpj)
                                                 .Get();
            return response.Models.FirstOrDefault();
        }

        public async Task<Empresa?> GetByUuidAsync(Guid uuid)
        {
            var resp = await _supabaseClient.From<Empresa>()
                .Filter("uuid", Supabase.Postgrest.Constants.Operator.Equals, uuid.ToString())
                .Get();

            return resp.Models.FirstOrDefault();
        }
    }
}
