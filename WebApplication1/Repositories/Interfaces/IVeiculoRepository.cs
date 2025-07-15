using Meucombustivel.Models; // Importa a sua Model Veiculo
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meucombustivel.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de acesso a dados da entidade Veiculo.
    /// Herda de IGenericRepository para operações CRUD básicas.
    /// </summary>
    public interface IVeiculoRepository : IGenericRepository<Veiculo>
    {
    
        Task<Veiculo?> GetByPlacaAsync(string placa);

  
        Task<IEnumerable<Veiculo>> GetVeiculosByEmpresaIdAsync(Guid empresaId);

    }
}