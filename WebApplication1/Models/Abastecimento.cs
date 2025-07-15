using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;



namespace Meucombustivel.Models
{
    [Table("abastecimentos")] 
    public class Abastecimento : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("veiculo_id")]
        public Guid VeiculoId { get; set; } 

        [Column("data")]
        public DateTime Data { get; set; }

        [Column("custo")]
        public decimal Custo { get; set; } 

        [Column("nota_fiscal_url")]
        public string NotaFiscalUrl { get; set; } 


        [Column("motorista_id")]
        public Guid MotoristaId { get; set; } 

        [Column("km_inicial")]
        public int KmInicial { get; set; } 
        [Column("localizacao")]
        public string Localizacao { get; set; } 

        [Column("litros")]
        public decimal Litros { get; set; } 

        [Column("tipo_combustivel")]
        public string TipoCombustivel { get; set; } 
    }
}