namespace Meucombustivel.Dtos.Motorista
{

    public class CreateMotoristaDto
    {
    
        public string NumeroCnh { get; set; } 
        public DateTime ValidadeCnh { get; set; }
        public string CategoriaCnh { get; set; }
    }
    public class ReadMotoristaDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string Cpf { get; set; } = string.Empty;
        
        public string Email { get; set; } = string.Empty;
        public string NumeroCnh { get; set; } = string.Empty;
        public DateTime ValidadeCnh { get; set; }
        public string CategoriaCnh { get; set; } = string.Empty;
    }

    public class UpdateMotoristaDto
    {

        public string? Nome { get; set; }

        public string ?Cpf { get; set; }
        
        public string ?Email { get; set; }

        public string ?NumeroCnh { get; set; }

        public DateTime? ValidadeCnh { get; set; }
        
        public string? CategoriaCnh { get; set; }

    }
}
