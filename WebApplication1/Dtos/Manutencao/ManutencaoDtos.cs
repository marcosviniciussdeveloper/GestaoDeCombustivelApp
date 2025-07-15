namespace Meucombustivel.Dtos.Manutencao
{
   
       public class CreateManutencaoDto
        {
            public Guid IdVeiculo { get; set; }
            public DateTime Data { get; set; }
            public string TipoManutencao { get; set; }
            public string Descricao { get; set; }
            public double Custo { get; set; }
            public string NotaFiscalUrl { get; set; }
        }

        public class UpdateManutencaoDto
        {
            public DateTime? Data { get; set; }
            public string? Tipo_Manutencao { get; set; }

            public string? Descricao { get; set; }

            public double? Custo { get; set; }

            public string? Nota_Fiscal_Url { get; set; }

        }

       public class ReadManutencaoDto
        {
            public Guid Id { get; set; }
            public string PlacaVeiculo { get; set; }
            public DateTime Data { get; set; }
            public string TipoManutencao { get; set; }
            public string Descricao { get; set; }
            public decimal Custo { get; set; }
            public string? Nota_Fiscal_Url { get; set; }
            public DateTime CreatedAt { get; set; }
        }

    }

















