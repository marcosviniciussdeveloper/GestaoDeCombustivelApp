using System.Security.Cryptography.X509Certificates;

namespace Meucombustivel.Dtos.Motorista
{

    public class CreateMotoristaDto
    {

        public string NumeroCnh { get; set; }
        public DateTime ValidadeCnh { get; set; }
        public string CategoriaCnh { get; set; }
    }
   public record ReadMotoristaDto
{
    public Guid MotoristaId { get; init; }
    public string? Nome { get; set; }     
    public string? Email { get; set; }   
    public string? Cpf { get; set; }    
    public string? NumeroCnh { get; init; }
    public DateTime? ValidadeCnh { get; init; }
    public string? CategoriaCnh { get; init; }
    public string? StatusVinculo { get; set; } 
}

    public class UpdateMotoristaDto
{

    public string? Nome { get; set; }

    public string? Cpf { get; set; }

    public string? Email { get; set; }

    public string? NumeroCnh { get; set; }

    public DateTime? ValidadeCnh { get; set; }

    public string? CategoriaCnh { get; set; }

}
}
