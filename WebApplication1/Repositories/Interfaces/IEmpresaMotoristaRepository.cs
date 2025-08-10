using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Repositories.Interfaces
{
    public interface IEmpresaMotoristaRepository
    {
        Task VincularAsync(Guid empresaId, Guid motoristaUsuarioId, string status = "ativo");

        Task DesvincularAsync(Guid empresaId, Guid motoristaUsuarioId);

        Task<bool> ExisteVinculoAsync(Guid empresaId, Guid motoristaUsuarioId);

  
        Task<EmpresaMotorista?> ObterAsync(Guid empresaId, Guid motoristaUsuarioId);

       
        Task AtualizarStatusAsync(Guid empresaId, Guid motoristaUsuarioId, string status);

     
        Task<IReadOnlyList<Guid>> ListarMotoristasIdsPorEmpresaAsync(Guid empresaId);

        Task<IReadOnlyList<Guid>> ListarEmpresasIdsPorMotoristaAsync(Guid motoristaUsuarioId);
    }
}
