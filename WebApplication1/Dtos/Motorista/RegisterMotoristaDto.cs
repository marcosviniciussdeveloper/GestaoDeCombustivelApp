using System;
using System.ComponentModel.DataAnnotations;

namespace Meucombustivel.Dtos.Motorista
{
    /// <summary>
    /// DTO para registrar um novo motorista, combinando dados de usuário e de CNH.
    /// </summary>
    public class RegisterMotoristaDto
    {
        [Required(ErrorMessage = "Nome do Motorista é obrigatório.")]
        [StringLength(100, ErrorMessage = "Nome do Motorista não pode exceder 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "CPF do Motorista é obrigatório.")]
        [StringLength(14, MinimumLength = 11, ErrorMessage = "CPF deve ter entre 11 e 14 caracteres (incluindo formatação).")]
        public string Cpf { get; set; }

        [Required(ErrorMessage = "Email do Motorista é obrigatório.")]
        [EmailAddress(ErrorMessage = "Formato de email inválido para o motorista.")]
        [StringLength(255, ErrorMessage = "Email do Motorista não pode exceder 255 caracteres.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha do Motorista é obrigatória.")]
        [StringLength(50, MinimumLength = 6, ErrorMessage = "A senha deve ter entre 6 e 50 caracteres.")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Número da CNH é obrigatório.")]
        [StringLength(20, ErrorMessage = "Número da CNH não pode exceder 20 caracteres.")]
        public string NumeroCnh { get; set; }

        [Required(ErrorMessage = "Validade da CNH é obrigatória.")]
        public DateTime ValidadeCnh { get; set; }

        [Required(ErrorMessage = "Categoria da CNH é obrigatória.")]
        [StringLength(5, ErrorMessage = "Categoria da CNH não pode exceder 5 caracteres.")]
        public string CategoriaCnh { get; set; }

        public Guid? EmpresaId { get; set; } 
    }
}
