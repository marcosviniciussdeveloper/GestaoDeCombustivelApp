using System.ComponentModel.DataAnnotations;

namespace Meucombustivel.Dtos.Empresa
{
    /// <summary>
    /// DTO para registrar uma nova empresa e seu primeiro usuário administrador.
    /// </summary>
    public class RegisterCompanyAndAdminDto
    {

        [Required(ErrorMessage = "Razão Social é obrigatória.")]
        [StringLength(255, ErrorMessage = "Razão Social não pode exceder 255 caracteres.")]
        public string RazaoSocial { get; set; }

        [Required(ErrorMessage = "CNPJ é obrigatório.")]
        [StringLength(18, MinimumLength = 14, ErrorMessage = "CNPJ deve ter entre 14 e 18 caracteres (incluindo formatação).")]
        public string Cnpj { get; set; }

        [Required(ErrorMessage = "Nome Fantasia é obrigatório.")]
        [StringLength(255, ErrorMessage = "Nome Fantasia não pode exceder 255 caracteres.")]
        public string NomeFantasia { get; set; }

        [Required(ErrorMessage = "Email de contato da Empresa é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido para a empresa.")]
        [StringLength(255, ErrorMessage = "Email de contato da Empresa não pode exceder 255 caracteres.")]
        public string EmailEmpresa { get; set; }

        [StringLength(20, ErrorMessage = "Telefone não pode exceder 20 caracteres.")]
        public string? Telefone { get; set; }

        [StringLength(500, ErrorMessage = "Endereço não pode exceder 500 caracteres.")]
        public string? Endereco { get; set; }

        [Required(ErrorMessage = "Nome do Administrador é obrigatório.")]
        [StringLength(100, ErrorMessage = "Nome do Administrador não pode exceder 100 caracteres.")]
        public string AdminNome { get; set; }

        [Required(ErrorMessage = "CPF do Administrador é obrigatório.")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "CPF deve ter entre 11 e 14 caracteres (incluindo formatação).")]
        public string AdminCpf { get; set; }

        [Required(ErrorMessage = "Email do Administrador é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido para o administrador.")]
        [StringLength(255, ErrorMessage = "Email do Administrador não pode exceder 255 caracteres.")]
        public string AdminEmail { get; set; }

        [Required(ErrorMessage = "Senha do Administrador é obrigatória.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 50 caracteres.")]
        public string AdminSenha { get; set; }
    }
}