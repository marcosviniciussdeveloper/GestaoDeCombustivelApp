namespace Meucombustivel.Dtos.Empresa
{

    public class CreateEmpresaDto
    {


        public string RazaoSocial { get; set; }

        public string Cnpj { get; set; }

        public string NomeFantasia { get; set; }

        public string Email { get; set; }

      

        public string Telefone { get; set; }

        public string Endereco { get; set; }
    }
    public class ReadEmpresaDto
    {
        public Guid Id { get; set; }
        public string RazaoSocial { get; set; }
        public string NomeFantasia { get; set; }
        public string Cnpj { get; set; }
        public string Email { get; set; }
        public string Telefone { get; set; }
        public string Endereco { get; set; }
        public DateTime Created_At { get; set; }
    }

    public class UpdateEmpresaDto
    {
        public string? Razao_Social { get; set; }
        public string? Cnpj { get; set; }
        public string? NomeFantasia { get; set; }
        public string? Email { get; set; }
        public string? Telefone { get; set; }

        public string? Endereco { get; set; }


    }
}

