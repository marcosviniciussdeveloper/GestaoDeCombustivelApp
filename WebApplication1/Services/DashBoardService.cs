using System.Security.Cryptography.X509Certificates;
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

            _http.DefaultRequestHeaders.Add("apikey", _supabase.ApiKey);
            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {_supabase.ApiKey}");
        }

        public async Task<DashboardDto> ObterDashboardAsync(Guid? empresaId = null, DateTime? de = null, DateTime? ate = null)
        {
            string baseUrl = _supabase.Url;
            string filtroEmpresa = empresaId.HasValue ? $"&empresa_id=eq.{empresaId}" : string.Empty;

            var abastecimentos = await _http.GetFromJsonAsync<List<JsonElement>>($"/abastecimentos?select=*&limit=10000{filtroEmpresa}");
            var manutencoes = await _http.GetFromJsonAsync<List<JsonElement>>($"manutencoes?select=*&limit=10000{filtroEmpresa}");
            var veiculos = await _http.GetFromJsonAsync<List<JsonElement>>($"/veiculos?select=*&limit=10000{filtroEmpresa}");
            var motoristas = await _http.GetFromJsonAsync<List<JsonElement>>($"/motoristas?select=*&limit=10000{filtroEmpresa}");

            DateTime now = DateTime.UtcNow;
            DateTime inicioMes = new DateTime(now.Year, now.Month, 1);
            DateTime inicioSemana = now.AddDays(-7);
            DateTime inicioAno = new DateTime(now.Year, 1, 1);

            Func<JsonElement, DateTime?> getDate = (item) =>
            {
                if (item.TryGetProperty("data", out var dateProp) && dateProp.ValueKind == JsonValueKind.String)
                {
                    return DateTime.TryParse(dateProp.GetString(), out var dt) ? dt : null;
                }
                return null;
            };

            Func<IEnumerable<JsonElement>, DateTime, DateTime, int> countIn = (list, start, end) => list.Count(e =>
            {
                var dt = getDate(e);
                return dt.HasValue && dt >= start && dt <= end;
            });

            Func<IEnumerable<JsonElement>, DateTime, DateTime, decimal> sumCostIn = (list, start, end) =>
                list.Where(e =>
                {
                    var dt = getDate(e);
                    return dt.HasValue && dt >= start && dt <= end;
                })
                .Select(e => e.TryGetProperty("custo", out var c) ? c.GetDecimal() : 0)
                .Sum();

            Func<IEnumerable<JsonElement>, DateTime, DateTime, decimal> sumLitrosIn = (list, start, end) =>
                list.Where(e =>
                {
                    var dt = getDate(e);
                    return dt.HasValue && dt >= start && dt <= end;
                })
                .Select(e => e.TryGetProperty("litros", out var l) ? l.GetDecimal() : 0)
                .Sum();

            const decimal precoMedioMercado = 6.00m;

            decimal litrosMes = sumLitrosIn(abastecimentos, inicioMes, now);
            decimal litrosDia = sumLitrosIn(abastecimentos, now.Date, now);
            decimal litrosSemana = sumLitrosIn(abastecimentos, inicioSemana, now);
            decimal litrosAno = sumLitrosIn(abastecimentos, inicioAno, now);
            decimal litrosTotal = sumLitrosIn(abastecimentos, DateTime.MinValue, now);

            decimal custoRealMes = sumCostIn(abastecimentos, inicioMes, now);
            decimal custoRealDia = sumCostIn(abastecimentos, now.Date, now);
            decimal custoRealSemana = sumCostIn(abastecimentos, inicioSemana, now);
            decimal custoRealAno = sumCostIn(abastecimentos, inicioAno, now);
            decimal custoRealTotal = sumCostIn(abastecimentos, DateTime.MinValue, now);

            decimal economiaMes = litrosMes * precoMedioMercado - custoRealMes;
            decimal economiaDia = litrosDia * precoMedioMercado - custoRealDia;
            decimal economiaSemana = litrosSemana * precoMedioMercado - custoRealSemana;
            decimal economiaAno = litrosAno * precoMedioMercado - custoRealAno;
            decimal economiaTotal = litrosTotal * precoMedioMercado - custoRealTotal;

            var dto = new DashboardDto
            {
                EconomiaTotal = economiaTotal,
                EconomiaMensal = economiaMes,
                EconomiaDiaria = economiaDia,
                EconomiaSemanal = economiaSemana,
                EconomiaAnual = economiaAno,

                Abastecimentos = abastecimentos?.Count ?? 0,
                AbastecimentosMensal = countIn(abastecimentos, inicioMes, now),
                AbastecimentosDiario = countIn(abastecimentos, now.Date, now),
                AbastecimentosAnual = countIn(abastecimentos, inicioAno, now),
                AbastecimentosSemanal = countIn(abastecimentos, inicioSemana, now),

                Manutencao = manutencoes?.Count ?? 0,
                ManutencaoMensal = countIn(manutencoes, inicioMes, now),
                ManutencaoDiaria = countIn(manutencoes, now.Date, now),
                ManutencaoSemanal = countIn(manutencoes, inicioSemana, now),
                ManutencaoAnual = countIn(manutencoes, inicioAno, now),

                VeiculosTotal = veiculos?.Count ?? 0,
                VeiculosAtivos = veiculos?.Count ?? 0,
                VeiculosInativos = 0,

                MotoristasAtivos = motoristas?.Count ?? 0,
            };

            return dto;
        }

        public async Task<List<DashboardMensalDto>> ObterDadosMensaisAsync(Guid? empresaId = null)
        {
            string baseUrl = _supabase.Url;
            string filtroEmpresa = empresaId.HasValue ? $"&empresa_id=eq.{empresaId}" : string.Empty;

            var abastecimentos = await _http.GetFromJsonAsync<List<JsonElement>>($"bastecimentos?select=*&limit=10000{filtroEmpresa}");

            Func<JsonElement, DateTime?> getDate = (item) =>
            {
                if (item.TryGetProperty("data", out var dateProp) && dateProp.ValueKind == JsonValueKind.String)
                {
                    return DateTime.TryParse(dateProp.GetString(), out var dt) ? dt : null;
                }
                return null;
            };

            var dadosPorMes = abastecimentos
                .Where(e => getDate(e).HasValue)
                .GroupBy(e => getDate(e).Value.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .Select(g =>
                {
                    var mes = DateTime.ParseExact(g.Key, "yyyy-MM", null);
                    var totalLitros = g.Select(e => e.TryGetProperty("litros", out var l) ? l.GetDecimal() : 0).Sum();
                    var totalGasto = g.Select(e => e.TryGetProperty("custo", out var c) ? c.GetDecimal() : 0).Sum();
                    var precoMedioMercado = 6.00m;

                    return new DashboardMensalDto
                    {
                        Mes = mes.ToString("MMM", new System.Globalization.CultureInfo("pt-BR")),
                        Gasto = totalGasto,
                        Economia = totalLitros * precoMedioMercado - totalGasto
                    };
                })
                .ToList();

            return dadosPorMes;
        }






    }
}
