
using Supabase.Postgrest;
           
using WebApplication1.Models;
using WebApplication1.Repositories.Interfaces;
using static Supabase.Postgrest.Constants;

namespace WebApplication1.Repositories
{
    public class EmpresaMotoristaRepository : IEmpresaMotoristaRepository
    {
        private readonly Supabase.Client _supabase;
        public EmpresaMotoristaRepository(Supabase.Client supabase) => _supabase = supabase;

        public async Task VincularAsync(Guid empresaId, Guid motoristaUsuarioId, string status = "ativo")
        {
            var vinculo = new EmpresaMotorista
            {
                EmpresaId = empresaId,
                MotoristaUsuarioId = motoristaUsuarioId,
                Status = status
            };

            // UPSERT compatível com todas as versões do client
            var opts = new QueryOptions
            {
                Upsert = true,
                OnConflict = "empresa_id,motorista_usuario_id", // PK composta/índice único
                Returning = QueryOptions.ReturnType.Representation
            };

            var resp = await _supabase
                .From<EmpresaMotorista>()
                .Insert(vinculo, opts);

            if (!resp.ResponseMessage.IsSuccessStatusCode)
            {
                var body = await resp.ResponseMessage.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha no upsert empresa_motorista: {resp.ResponseMessage.StatusCode} - {body}");
            }
        }
      public async Task DesvincularAsync(Guid empresaId, Guid motoristaUsuarioId)
{
    await _supabase
        .From<EmpresaMotorista>()
        .Filter("empresa_id", Operator.Equals, empresaId)
        .Filter("motorista_usuario_id", Operator.Equals, motoristaUsuarioId)
        .Delete();


    var check = await _supabase
        .From<EmpresaMotorista>()
        .Filter("empresa_id", Operator.Equals, empresaId)
        .Filter("motorista_usuario_id", Operator.Equals, motoristaUsuarioId)
        .Limit(1)
        .Get();

    if (check.Models.Any())
        throw new InvalidOperationException("Falha ao desvincular (registro ainda existe).");
}

        public async Task<bool> ExisteVinculoAsync(Guid empresaId, Guid motoristaUsuarioId)
        {
            var r = await _supabase
                .From<EmpresaMotorista>()
                .Where(x => x.EmpresaId == empresaId && x.MotoristaUsuarioId == motoristaUsuarioId)
                .Limit(1)
                .Get();

            return r.Models.Any();
        }

        public async Task<EmpresaMotorista?> ObterAsync(Guid empresaId, Guid motoristaUsuarioId)
        {
            var r = await _supabase
                .From<EmpresaMotorista>()
                .Where(x => x.EmpresaId == empresaId && x.MotoristaUsuarioId == motoristaUsuarioId)
                .Limit(1)
                .Get();

            return r.Models.FirstOrDefault();
        }

        public async Task AtualizarStatusAsync(Guid empresaId, Guid motoristaUsuarioId, string status)
        {
            var update = new EmpresaMotorista
            {
                EmpresaId = empresaId,
                MotoristaUsuarioId = motoristaUsuarioId,
                Status = status
            };

            var resp = await _supabase
                .From<EmpresaMotorista>()
                .Where(x => x.EmpresaId == empresaId && x.MotoristaUsuarioId == motoristaUsuarioId)
                .Update(update);

            if (!resp.ResponseMessage.IsSuccessStatusCode)
            {
                var body = await resp.ResponseMessage.Content.ReadAsStringAsync();
                throw new InvalidOperationException($"Falha ao atualizar status: {resp.ResponseMessage.StatusCode} - {body}");
            }
        }

        public async Task<IReadOnlyList<Guid>> ListarMotoristasIdsPorEmpresaAsync(Guid empresaId)
        {
            var r = await _supabase
                .From<EmpresaMotorista>()
                .Where(x => x.EmpresaId == empresaId)
                .Get();

            return r.Models.Select(x => x.MotoristaUsuarioId).ToList();
        }

        public async Task<IReadOnlyList<Guid>> ListarEmpresasIdsPorMotoristaAsync(Guid motoristaUsuarioId)
        {
            var r = await _supabase
                .From<EmpresaMotorista>()
                .Where(x => x.MotoristaUsuarioId == motoristaUsuarioId)
                .Get();

            return r.Models.Select(x => x.EmpresaId).ToList();
        }

        public async Task<IReadOnlyList<EmpresaMotorista>> ListarVinculosPorEmpresaAsync(Guid empresaId)
        {
            var resp = await _supabase
                .From<EmpresaMotorista>()
                .Filter("empresa_id", Operator.Equals, empresaId)
                .Get();

            return resp.Models; 
        }
    }
}
