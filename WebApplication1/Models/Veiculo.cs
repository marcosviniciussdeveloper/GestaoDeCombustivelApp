
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Meucombustivel.Models
{

    [Table("veiculos")]
    public class Veiculo : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }
     
        [Column("empresa_id")]
        public Guid EmpresaId { get; set; }


        [Column("placa")]
        public string Placa { get; set; }


        [Column("modelo")]
        public string Modelo { get; set; }

        [Column("tipo_combustivel")]

        public string TipoCombustivel { get; set; }

        [Column("quilometragem_atual")]
        public double QuilometragemAtual { get; set; }

        [Column("categoria")]
        public string Categoria { get; set; }

        [Column("ano")]
        public int Ano { get; set; }

        [Column("marca")]

        public string Marca { get; set; }

    


 

    }

}
