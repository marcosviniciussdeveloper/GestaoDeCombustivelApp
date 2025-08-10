// remova o using Castle.Components.DictionaryAdapter; (não é necessário)

public class DashboardDto
{
    public decimal EconomiaTotal { get; set; }
    public decimal EconomiaMensal { get; set; }
    public decimal EconomiaDiaria { get; set; }
    public decimal EconomiaAnual { get; set; }
    public decimal EconomiaSemanal { get; set; }

    public int Abastecimentos { get; set; }
    public int AbastecimentosMensal { get; set; }
    public int AbastecimentosDiario { get; set; }
    public int AbastecimentosAnual { get; set; }
    public int AbastecimentosSemanal { get; set; }

    public int Manutencao { get; set; }
    public int ManutencaoMensal { get; set; }
    public int ManutencaoDiaria { get; set; }
    public int ManutencaoAnual { get; set; }
    public int ManutencaoSemanal { get; set; }

    public int VeiculosAtivos { get; set; }
    public int VeiculosInativos { get; set; }
    public int VeiculosTotal { get; set; }

    public int MotoristasAtivos { get; set; }

    public class DashboardMensalDto
    {
        public string Mes { get; set; } = string.Empty;

        
        public decimal TotalCusto { get; set; }     
        public decimal TotalLitros { get; set; }

        public decimal Gasto
        {
            get => TotalCusto;
            set => TotalCusto = value;
        }

        public decimal Economia { get; set; }
    }
}
