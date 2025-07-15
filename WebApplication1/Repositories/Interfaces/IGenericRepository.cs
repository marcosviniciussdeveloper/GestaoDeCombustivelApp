using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Meucombustivel.Repositories.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task<TEntity> AddAsync(TEntity entity); 
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(Guid id);
    }
}
