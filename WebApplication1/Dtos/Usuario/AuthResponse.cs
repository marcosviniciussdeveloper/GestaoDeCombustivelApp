// Dtos/Auth/AuthResponseDto.cs
using Meucombustivel.Dtos.Usuario;

public sealed class AuthResponseDto
{
    public string Token { get; set; } = default!;
    public ReadUsuarioDto User { get; set; } = default!;


}

public sealed class AuthUserDto
{
   
}
