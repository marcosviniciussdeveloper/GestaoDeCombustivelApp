
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Meucombustivel.Models
{

    [Table("manutencoes")]
    public class Manutencoes: BaseModel
    {
        [PrimaryKey("id")]
         public Guid Id { get; set; }


        [Column("id_veiculo")]
        public Guid IdVeiculo { get; set; }

        [Column("data")]
        public DateTime Data { get; set; }

        [Column("tipo_manutencao")]
        public string TipoManutencao { get; set; }

        [Column("descricao")]
        public string Descricao { get; set; }

        [Column("custo")]
        public double Custo { get; set; }

        [Column("nota_fiscal_url")]
        public  string Nota_Fiscal_Url { get; set; }

     
    }
}
