using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;

namespace Meucombustivel.Models;
    [Table("motoristas")]
    public class Motorista : BaseModel
{
    [PrimaryKey("usuario_id")] // A chave primária da tabela motoristas é o ID do usuário
    [Column("usuario_id")]
    public Guid UsuarioId { get; set; } 

    [Column("numero_cnh")]
    public string NumeroCnh { get; set; }

    [Column("categoria_cnh")]
    public string CategoriaCnh { get; set; }

    [Column("status")]
    public bool? Status {get;set;}

    [Column("validade_cnh")]
    public DateTime ValidadeCnh { get; set; }

}