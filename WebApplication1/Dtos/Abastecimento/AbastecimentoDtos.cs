namespace Meucombustivel.Dtos.Abastecimento
{
   
        public class CreateAbastecimentoDto
        {

            public Guid VeiculoId { get; set; }

            public Guid MotoristaId { get; set; }
            public DateTime Data { get; set; }
            public string TipoCombustivel { get;set;}
            public double Custo { get; set; }
            public string NotaFiscalUrl { get; set; }
            public string? Localização { get; set; }
            public double KmInicial { get; set; }
            public decimal Litros { get; set; }
        }
        public class ReadAbastecimentoDto
        {
            public Guid Id { get; set; }
            public string PlacaVeiculo { get; set; }
            public string NomeMotorista { get; set; }
            public DateTime Data { get; set; }
            public decimal Custo { get; set; }
            public decimal Litros { get; set; }
            public decimal KmInicial { get; set; }

            public string NotaFiscalUrl { get; set; }
            public string Localizacao { get; set; }
            public DateTime Created_At { get; set; }
        }
        public class UpdateAbastecimentoDto
        {

            public Guid Id { get; set; }
            public DateTime? Data { get; set; }
            public decimal? Custo { get; set; }
            public string? Nota_Fiscal_Url { get; set; }
            public string? Localização { get; set; }
            public decimal? KmInicial { get; set; }
            public double? Litros { get; set; }

        }
    }

