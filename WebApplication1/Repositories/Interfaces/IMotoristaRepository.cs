using Meucombustivel.Dtos.Motorista;
using Meucombustivel.Models; // Importa a sua Model Motorista
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meucombustivel.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de acesso a dados da entidade Motorista.
    /// Herda de IGenericRepository para operações CRUD básicas.
    /// </summary>
    public interface IMotoristaRepository : IGenericRepository<Motorista>
    {
        /// <summary>
        /// Busca um Motorista pelo ID do Usuário associado a ele (que é a chave primária da tabela Motorista).
        /// </summary>
        /// <param name="usuarioId">O ID do usuário associado ao motorista.</param>
        /// <returns>O objeto Motorista encontrado ou null se não existir.</returns>
      

        Task <Motorista> StatusMotoristaAsync(Guid usuarioId, bool novoStatus);

        Task<IReadOnlyList<ReadMotoristaDto>> ListarPorEmpresaAsync(Guid empresaId);

   
    }
}