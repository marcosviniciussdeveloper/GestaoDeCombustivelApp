using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System.ComponentModel.DataAnnotations.Schema;
using ColumnAttribute = Supabase.Postgrest.Attributes.ColumnAttribute;
using TableAttribute = System.ComponentModel.DataAnnotations.Schema.TableAttribute;

namespace WebApplication1.Models.View
{
    [Table(" vw_motorista_por_empresa")]
    public class VwMotoristaEmpresa : BaseModel
{
    [PrimaryKey("empresa_id", false)] public Guid EmpresaId { get; set; }
    [Column("motorista_id")]          public Guid MotoristaId { get; set; }
    [Column("nome")]                  public string? Nome { get; set; }
    [Column("email")]                 public string? Email { get; set; }
    [Column("cpf")]                   public string? Cpf { get; set; }
    [Column("numero_cnh")]            public string? NumeroCnh { get; set; }
    [Column("validade_cnh")]          public DateTime? ValidadeCnh { get; set; }
    [Column("categoria_cnh")]         public string? CategoriaCnh { get; set; }
    [Column("status")]                public string? Status { get; set; }
}
}
