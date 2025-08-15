// Dtos/Auth/AuthResponseDto.cs
using Meucombustivel.Dtos.Usuario;

public sealed class AuthResponseDto
{
    public string Token { get; set; } = default!;
    public ReadUsuarioDto User { get; set; } = default!;
}

public sealed class AuthUserDto
{
    public Guid Id { get; init; }
    public string Nome { get; init; } = default!;
    public string Email { get; init; } = default!;
    public string TipoUsuario { get; init; } = default!;
    public Guid? EmpresaId { get; init; }
}
