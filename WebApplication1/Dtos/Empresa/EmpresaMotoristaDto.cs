namespace WebApplication1.Dtos.Empresa
{
    public record EmpresaMotoristaDto(
     Guid EmpresaId,
     Guid MotoristaUsuarioId,
     string Status
 );
}
