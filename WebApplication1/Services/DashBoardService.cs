using System.Text.Json;
using Meucombustivel.Models;
using Microsoft.Extensions.Options;
using WebApplication1.Services.Interface;
using static DashboardDto;

namespace WebApplication1.Services
{
    public class SupabaseSettings
    {
        public string Url { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
    }

    public class DashboardService : IDashBoardService
    {
        private readonly HttpClient _http;
        private readonly SupabaseSettings _supabase;

        public DashboardService(HttpClient http, IOptions<SupabaseSettings> supabaseOptions)
        {
            _http = http;
            _supabase = supabaseOptions.Value;

            // evita duplicar headers se o HttpClient for reutilizado
            _http.DefaultRequestHeaders.Remove("apikey");
            _http.DefaultRequestHeaders.Remove("Authorization");
            _http.DefaultRequestHeaders.Add("apikey", _supabase.ApiKey);
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_supabase.ApiKey}");
        }

        // helper para montar query e buscar lista no Supabase
        private async Task<List<JsonElement>> FetchAsync(
            string table,
            bool aplicarFiltroEmpresa,
            Guid? empresaId,
            DateTime? de,
            DateTime? ate)
        {
            var qs = new List<string> { "select=*", "limit=10000" };

            if (aplicarFiltroEmpresa && empresaId.HasValue)
                qs.Add($"empresa_id=eq.{empresaId}");

            // filtros de período por coluna "data" (quando existir)
            if (de.HasValue) qs.Add($"data=gte.{de.Value.ToUniversalTime():O}");
            if (ate.HasValue) qs.Add($"data=lte.{ate.Value.ToUniversalTime():O}");

            var url = $"{table}?{string.Join("&", qs)}";

            try
            {
                return await _http.GetFromJsonAsync<List<JsonElement>>(url) ?? new List<JsonElement>();
            }
            catch
            {
                // se falhar por coluna inexistente (ex.: empresa_id não existe em motoristas), tenta sem o filtro de empresa
                if (aplicarFiltroEmpresa)
                {
                    var semEmpresa = qs.Where(p => !p.StartsWith("empresa_id=", StringComparison.OrdinalIgnoreCase)).ToList();
                    url = $"{table}?{string.Join("&", semEmpresa)}";
                    return await _http.GetFromJsonAsync<List<JsonElement>>(url) ?? new List<JsonElement>();
                }
                throw;
            }
        }

        public async Task<DashboardDto> ObterDashboardAsync(Guid? empresaId = null, DateTime? de = null, DateTime? ate = null)
        {
            // busca dados (usa BaseAddress; paths relativos)
            var abastecimentos = await FetchAsync("abastecimentos", aplicarFiltroEmpresa: true, empresaId, de, ate);
            var manutencoes    = await FetchAsync("manutencoes",    aplicarFiltroEmpresa: true, empresaId, de, ate);
            var veiculos       = await FetchAsync("veiculos",       aplicarFiltroEmpresa: true, empresaId, de, ate);
            var motoristas     = await FetchAsync("motoristas",     aplicarFiltroEmpresa: false, empresaId, de, ate);

            DateTime now = DateTime.UtcNow;
            DateTime inicioMes    = new DateTime(now.Year, now.Month, 1);
            DateTime inicioSemana = now.AddDays(-7);
            DateTime inicioAno    = new DateTime(now.Year, 1, 1);

            DateTime? GetDate(JsonElement item)
            {
                if (item.TryGetProperty("data", out var dateProp) && dateProp.ValueKind == JsonValueKind.String)
                    return DateTime.TryParse(dateProp.GetString(), out var dt) ? dt : null;
                return null;
            }

            int CountIn(IEnumerable<JsonElement> list, DateTime start, DateTime end) =>
                list.Count(e => { var dt = GetDate(e); return dt.HasValue && dt >= start && dt <= end; });

            decimal SumIn(IEnumerable<JsonElement> list, DateTime start, DateTime end, string campo) =>
                list.Where(e => { var dt = GetDate(e); return dt.HasValue && dt >= start && dt <= end; })
                    .Select(e => e.TryGetProperty(campo, out var v) ? v.GetDecimal() : 0m)
                    .Sum();

            const decimal precoMedioMercado = 6.00m;

            decimal litrosMes     = SumIn(abastecimentos, inicioMes,    now, "litros");
            decimal litrosDia     = SumIn(abastecimentos, now.Date,     now, "litros");
            decimal litrosSemana  = SumIn(abastecimentos, inicioSemana, now, "litros");
            decimal litrosAno     = SumIn(abastecimentos, inicioAno,    now, "litros");
            decimal litrosTotal   = SumIn(abastecimentos, DateTime.MinValue, now, "litros");

            decimal custoRealMes     = SumIn(abastecimentos, inicioMes,    now, "custo");
            decimal custoRealDia     = SumIn(abastecimentos, now.Date,     now, "custo");
            decimal custoRealSemana  = SumIn(abastecimentos, inicioSemana, now, "custo");
            decimal custoRealAno     = SumIn(abastecimentos, inicioAno,    now, "custo");
            decimal custoRealTotal   = SumIn(abastecimentos, DateTime.MinValue, now, "custo");

            var dto = new DashboardDto
            {
                EconomiaTotal   = litrosTotal  * precoMedioMercado - custoRealTotal,
                EconomiaMensal  = litrosMes    * precoMedioMercado - custoRealMes,
                EconomiaDiaria  = litrosDia    * precoMedioMercado - custoRealDia,
                EconomiaSemanal = litrosSemana * precoMedioMercado - custoRealSemana,
                EconomiaAnual   = litrosAno    * precoMedioMercado - custoRealAno,

                Abastecimentos        = abastecimentos?.Count ?? 0,
                AbastecimentosMensal  = CountIn(abastecimentos, inicioMes,    now),
                AbastecimentosDiario  = CountIn(abastecimentos, now.Date,     now),
                AbastecimentosAnual   = CountIn(abastecimentos, inicioAno,    now),
                AbastecimentosSemanal = CountIn(abastecimentos, inicioSemana, now),

                Manutencao        = manutencoes?.Count ?? 0,
                ManutencaoMensal  = CountIn(manutencoes, inicioMes,    now),
                ManutencaoDiaria  = CountIn(manutencoes, now.Date,     now),
                ManutencaoSemanal = CountIn(manutencoes, inicioSemana, now),
                ManutencaoAnual   = CountIn(manutencoes, inicioAno,    now),

                VeiculosTotal   = veiculos?.Count ?? 0,
                VeiculosAtivos  = veiculos?.Count ?? 0, // ajuste aqui caso tenha coluna de status
                VeiculosInativos = 0,

                MotoristasAtivos = motoristas?.Count ?? 0
            };

            return dto;
        }

        public async Task<List<DashboardMensalDto>> ObterDadosMensaisAsync(Guid? empresaId = null)
        {
            var abastecimentos = await FetchAsync("abastecimentos", aplicarFiltroEmpresa: true, empresaId, de: null, ate: null);

            DateTime? GetDate(JsonElement item)
            {
                if (item.TryGetProperty("data", out var dateProp) && dateProp.ValueKind == JsonValueKind.String)
                    return DateTime.TryParse(dateProp.GetString(), out var dt) ? dt : null;
                return null;
            }

            var dadosPorMes = abastecimentos
                .Where(e => GetDate(e).HasValue)
                .GroupBy(e => GetDate(e)!.Value.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var mes = DateTime.ParseExact(g.Key, "yyyy-MM", null);
                    var totalLitros = g.Select(e => e.TryGetProperty("litros", out var l) ? l.GetDecimal() : 0m).Sum();
                    var totalGasto  = g.Select(e => e.TryGetProperty("custo",  out var c) ? c.GetDecimal() : 0m).Sum();
                    const decimal precoMedioMercado = 6.00m;

                    return new DashboardMensalDto
                    {
                        Mes      = mes.ToString("MMM", new System.Globalization.CultureInfo("pt-BR")),
                        TotalCusto    = totalGasto,
                        TotalLitros = totalLitros,
                        Economia = totalLitros * precoMedioMercado - totalGasto
                    };
                })
                .ToList();

            return dadosPorMes;
        }
    }
}
