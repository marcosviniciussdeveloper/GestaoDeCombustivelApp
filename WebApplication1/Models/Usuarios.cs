
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Meucombustivel.Models
{
    [Table("usuarios")]
    public class Usuarios : BaseModel 
    {
        [PrimaryKey("id")]
        public Guid Id { get; set; } 

        [Column("empresa_id")]
        public Guid? EmpresaId { get; set; } 

        [Column("nome")]
        public string Nome { get; set; }                        

        [Column("tipo_usuario")]
        public string TipoUsuario { get; set; }  

        [Column("cpf")]
        public string Cpf { get; set; } 

        [Column("email")]
        public string Email { get; set; } 

        [Column("id_auth")]
        public Guid IdAuth { get; set; } 

     
    }
}
