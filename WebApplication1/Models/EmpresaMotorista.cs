
using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using System;

namespace WebApplication1.Models
{
    [Table("empresa_motorista")]
    public class EmpresaMotorista : BaseModel
    {
        [PrimaryKey("empresa_id", false)]
        public Guid EmpresaId { get; set; }

      
        [PrimaryKey("motorista_usuario_id", false)]
        public Guid MotoristaUsuarioId { get; set; }

        [Column("status")]
        public string? Status { get; set; }

        [Column("vinculado_em")]
        public DateTime? VinculadoEm { get; set; }
    }
}
