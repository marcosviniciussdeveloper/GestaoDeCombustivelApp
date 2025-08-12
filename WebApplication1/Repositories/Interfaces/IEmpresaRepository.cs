using Meucombustivel.Models; // Importa a sua Model Empresa
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meucombustivel.Repositories.Interfaces
{
    /// <summary>
    /// Define o contrato para operações de acesso a dados da entidade Empresa.
    /// Herda de IGenericRepository para operações CRUD básicas.
    /// </summary>
    public interface IEmpresaRepository : IGenericRepository<Empresa>
    {
     
        Task<Empresa?> GetByCnpjAsync(string cnpj);

         Task<Empresa?> GetByUuidAsync(Guid uuid);

    }
}