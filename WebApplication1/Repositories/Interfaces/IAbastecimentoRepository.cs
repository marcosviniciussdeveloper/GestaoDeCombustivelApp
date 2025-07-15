using Meucombustivel.Models;


namespace Meucombustivel.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de acesso a dados da entidade Abastecimento.
    /// Herda de IGenericRepository para operações CRUD básicas.
    /// </summary>
    public interface IAbastecimentoRepository : IGenericRepository<Abastecimento>
    {
     
        Task<IEnumerable<Abastecimento>> GetAbastecimentosByVeiculoIdAsync(Guid veiculoId);

        
        Task<IEnumerable<Abastecimento>> GetAbastecimentosByMotoristaIdAsync(Guid motoristaId);

        
    }
}