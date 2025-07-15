using Meucombustivel.Models;


namespace WebApplication1.ServicesAuth
{
    public interface ITokenService
    {
        string GenerateJwtToken(Usuarios usuario);

    }
}
