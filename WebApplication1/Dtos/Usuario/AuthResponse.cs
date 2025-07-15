namespace Meucombustivel.Dtos.Usuario
{
    public class AuthResponseDto
    {
        public ReadUsuarioDto Usuario { get; set; }
        public string AccessToken { get; set; }
        public string? RefreshToken { get; set; }
    }
}
