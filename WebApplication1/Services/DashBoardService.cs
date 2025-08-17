using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using Meucombustivel.Models;
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

    
        public DashboardService(HttpClient http)
        {
            _http = http;
        }

        // ------------------------- Helpers -------------------------

        private static DateTime? GetDate(JsonElement item)
        {
            if (item.TryGetProperty("data", out var dateProp) && dateProp.ValueKind == JsonValueKind.String)
            {
                if (DateTime.TryParse(dateProp.GetString(), out var dt)) return dt;
            }
            return null;
        }

        private static decimal GetDecimal(JsonElement e, string prop)
        {
            if (!e.TryGetProperty(prop, out var v)) return 0m;
            return v.ValueKind switch
            {
                JsonValueKind.Number => v.GetDecimal(),
                JsonValueKind.String => decimal.TryParse(v.GetString(), NumberStyles.Any, CultureInfo.InvariantCulture, out var d) ? d : 0m,
                _ => 0m
            };
        }

        private static int CountIn(IEnumerable<JsonElement> list, DateTime start, DateTime end) =>
            list.Count(e => { var dt = GetDate(e); return dt.HasValue && dt.Value >= start && dt.Value <= end; });

        private static decimal SumIn(IEnumerable<JsonElement> list, DateTime start, DateTime end, string campo) =>
            list.Where(e => { var dt = GetDate(e); return dt.HasValue && dt.Value >= start && dt.Value <= end; })
                .Select(e => GetDecimal(e, campo))
                .Sum();

        // ---------------------- Supabase Fetch ----------------------
        // aplicarFiltroData: use TRUE apenas em tabelas que possuam coluna "data"
        private async Task<List<JsonElement>> FetchAsync(
            string table,
            bool aplicarFiltroEmpresa,
            bool aplicarFiltroData,
            Guid? empresaId,
            DateTime? de,
            DateTime? ate)
        {
            var qs = new List<string> { "select=*", "limit=10000" };

            if (aplicarFiltroEmpresa && empresaId.HasValue)
                qs.Add($"empresa_id=eq.{empresaId}");

            if (aplicarFiltroData)
            {
                if (de.HasValue) qs.Add($"data=gte.{de.Value.ToUniversalTime():O}");
                if (ate.HasValue) qs.Add($"data=lte.{ate.Value.ToUniversalTime():O}");
            }

            var url = $"{table}?{string.Join("&", qs)}";

            try
            {
                return await _http.GetFromJsonAsync<List<JsonElement>>(url) ?? new List<JsonElement>();
            }
            catch
            {
                // Retry defensivo: remove filtros que podem não existir nesta tabela
                var qsRetry = qs
                    .Where(p =>
                        !p.StartsWith("empresa_id=", StringComparison.OrdinalIgnoreCase) &&
                        !p.StartsWith("data=gte.", StringComparison.OrdinalIgnoreCase) &&
                        !p.StartsWith("data=lte.", StringComparison.OrdinalIgnoreCase))
                    .ToList();

                url = $"{table}?{string.Join("&", qsRetry)}";
                return await _http.GetFromJsonAsync<List<JsonElement>>(url) ?? new List<JsonElement>();
            }
        }

        // --------------------- Dashboard (KPIs) ---------------------
        public async Task<DashboardDto> ObterDashboardAsync(
            Guid? empresaId = null, DateTime? de = null, DateTime? ate = null)
        {
            var abastecimentos = await FetchAsync("abastecimentos", aplicarFiltroEmpresa: true, aplicarFiltroData: true, empresaId, de, ate);
            var manutencoes    = await FetchAsync("manutencoes",    aplicarFiltroEmpresa: true, aplicarFiltroData: true, empresaId, de, ate);

           
            var veiculos   = await FetchAsync("veiculos",   aplicarFiltroEmpresa: true,  aplicarFiltroData: false, empresaId, de, ate);
            var motoristas = await FetchAsync("motoristas", aplicarFiltroEmpresa: false, aplicarFiltroData: false, empresaId, de, ate);

            var now          = DateTime.UtcNow;
            var inicioMes    = new DateTime(now.Year, now.Month, 1);
            var inicioSemana = now.AddDays(-7);
            var inicioAno    = new DateTime(now.Year, 1, 1);

            const decimal precoMedioMercado = 6.00m;

            var litrosMes     = SumIn(abastecimentos, inicioMes,    now, "litros");
            var litrosDia     = SumIn(abastecimentos, now.Date,     now, "litros");
            var litrosSemana  = SumIn(abastecimentos, inicioSemana, now, "litros");
            var litrosAno     = SumIn(abastecimentos, inicioAno,    now, "litros");
            var litrosTotal   = SumIn(abastecimentos, DateTime.MinValue, now, "litros");

            var custoMes      = SumIn(abastecimentos, inicioMes,    now, "custo");
            var custoDia      = SumIn(abastecimentos, now.Date,     now, "custo");
            var custoSemana   = SumIn(abastecimentos, inicioSemana, now, "custo");
            var custoAno      = SumIn(abastecimentos, inicioAno,    now, "custo");
            var custoTotal    = SumIn(abastecimentos, DateTime.MinValue, now, "custo");

            var dto = new DashboardDto
            {
                EconomiaTotal   = litrosTotal  * precoMedioMercado - custoTotal,
                EconomiaMensal  = litrosMes    * precoMedioMercado - custoMes,
                EconomiaDiaria  = litrosDia    * precoMedioMercado - custoDia,
                EconomiaSemanal = litrosSemana * precoMedioMercado - custoSemana,
                EconomiaAnual   = litrosAno    * precoMedioMercado - custoAno,

                Abastecimentos        = abastecimentos?.Count ?? 0,
                AbastecimentosMensal  = CountIn(abastecimentos, inicioMes,    now),
                AbastecimentosDiario  = CountIn(abastecimentos, now.Date,     now),
                AbastecimentosSemanal = CountIn(abastecimentos, inicioSemana, now),
                AbastecimentosAnual   = CountIn(abastecimentos, inicioAno,    now),

                Manutencao        = manutencoes?.Count ?? 0,
                ManutencaoMensal  = CountIn(manutencoes, inicioMes,    now),
                ManutencaoDiaria  = CountIn(manutencoes, now.Date,     now),
                ManutencaoSemanal = CountIn(manutencoes, inicioSemana, now),
                ManutencaoAnual   = CountIn(manutencoes, inicioAno,    now),

                VeiculosTotal    = veiculos?.Count ?? 0,
                VeiculosAtivos   = veiculos?.Count ?? 0, 
                VeiculosInativos = 0,

                MotoristasAtivos = motoristas?.Count ?? 0
            };

            return dto;
        }

        // ------------------- Dashboard (Mensal c/ range) -------------------
        public async Task<List<DashboardMensalDto>> ObterDadosMensaisAsync(
            Guid? empresaId = null, DateOnly? de = null, DateOnly? ate = null)
        {
           
            var hoje = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var ini = de  ?? new DateOnly(hoje.Year, hoje.Month, 1).AddMonths(-5);
            var fim = ate ?? new DateOnly(hoje.Year, hoje.Month, 1);
            if (ini > fim) (ini, fim) = (fim, ini);

            var dtIni = ini.ToDateTime(TimeOnly.MinValue);
            var dtFim = fim.ToDateTime(TimeOnly.MaxValue);


            var abastecimentos = await FetchAsync(
                "abastecimentos",
                aplicarFiltroEmpresa: true,
                aplicarFiltroData:   true,
                empresaId, dtIni, dtFim);

            
            var culturaPtBr = new CultureInfo("pt-BR");
            var meses = new List<DashboardMensalDto>();
            for (var d = new DateOnly(ini.Year, ini.Month, 1);
                 d <= new DateOnly(fim.Year, fim.Month, 1);
                 d = d.AddMonths(1))
            {
                meses.Add(new DashboardMensalDto
                {
                    Mes         = d.ToString("MMM", culturaPtBr),
                    TotalCusto  = 0m,
                    TotalLitros = 0m,
                    Economia    = 0m
                });
            }

         
            var primeiro = new DateTime(ini.Year, ini.Month, 1);
            var map = meses
                .Select((_, i) => new { Key = primeiro.AddMonths(i).ToString("yyyy-MM"), Idx = i })
                .ToDictionary(x => x.Key, x => x.Idx);

            const decimal precoMedioMercado = 6.00m;

           
            var grupos = abastecimentos
                .Select(e => new { Dt = GetDate(e), Litros = GetDecimal(e, "litros"), Custo = GetDecimal(e, "custo") })
                .Where(x => x.Dt.HasValue && x.Dt.Value >= dtIni && x.Dt.Value <= dtFim)
                .GroupBy(x => x.Dt!.Value.ToString("yyyy-MM"));

            foreach (var g in grupos)
            {
                if (!map.TryGetValue(g.Key, out var i)) continue;
                var litros = g.Sum(x => x.Litros);
                var custo  = g.Sum(x => x.Custo);
                meses[i].TotalLitros = litros;
                meses[i].TotalCusto  = custo;
                meses[i].Economia    = litros * precoMedioMercado - custo;
            }

            return meses;
        }

   
        public async Task<List<DashboardMensalDto>> ObterDadosMensaisAsync(Guid? empresaId = null)
        {
            var hoje = DateOnly.FromDateTime(DateTime.UtcNow.Date);
            var de  = new DateOnly(hoje.Year, hoje.Month, 1).AddMonths(-5);
            var ate = new DateOnly(hoje.Year, hoje.Month, 1);
            return await ObterDadosMensaisAsync(empresaId, de, ate);
        }
    }
}
