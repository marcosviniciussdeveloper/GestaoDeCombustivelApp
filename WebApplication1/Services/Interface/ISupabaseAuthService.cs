using Supabase.Gotrue;

namespace WebApplication1.Repositories.Interfaces
{
    public interface ISupabaseAuthService
    {

        Task<Session?> SignUp (string email, string Senha );
        Task<Session?> SignIn(string email, string senha);

    }
}
