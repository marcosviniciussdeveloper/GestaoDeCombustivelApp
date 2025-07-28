
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Meucombustivel.Models
{
    [Table("empresas")]
    public class Empresa : BaseModel
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; }

        [Column("razao_social")]
        public string RazaoSocial { get; set; }


        [Column("nome_fantasia")]
        public string NomeFantasia { get; set; }

        [Column("cnpj")]
        public string Cnpj { get; set; }

        [Column("email")]
        public string Email { get; set; }


        [Column("telefone")]
        public string Telefone { get; set; }

        [Column("endereco")]
        public string Endereco { get; set; }

    }
}

