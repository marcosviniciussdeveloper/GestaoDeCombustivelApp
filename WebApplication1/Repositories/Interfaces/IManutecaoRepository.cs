 using Meucombustivel.Models; // Importa a sua Model Manutencoes


namespace Meucombustivel.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de acesso a dados da entidade Manutencao.
    /// Herda de IGenericRepository para operações CRUD básicas.
    /// </summary>
    public interface IManutencaoRepository : IGenericRepository<Manutencoes> 
    {
      
        Task<IEnumerable<Manutencoes>> GetManutencoesByVeiculoIdAsync(Guid veiculoId);
    }

}