namespace Meucombustivel.Dtos.Usuario
{

    public class CreateUsuarioDto
    {   public Guid? EmpresaId { get; set; }
        public string Nome { get; set; }

        public string Cpf { get; set; }

        public string Email { get; set; }

        public string TipoUsuario { get; set; }

        public string Senha { get; set; }
    }
    public class UpdateUsuarioDto
    {
        public Guid? Id { get; set; }
        public string? Nome { get; set; }

        public string? Cpf { get; set; }
        public string? Email { get; set; }
        public string? TipoUsuario { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? Senha { get; set; }
    }

    public class ReadUsuarioDto
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public string TipoUsuario { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}



