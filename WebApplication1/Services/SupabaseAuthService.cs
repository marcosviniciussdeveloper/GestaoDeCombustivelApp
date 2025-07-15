using Supabase.Gotrue;
using WebApplication1.Repositories.Interfaces;

public class SupabaseAuthService : ISupabaseAuthService
{
    private readonly Supabase.Client _client;

    public SupabaseAuthService(Supabase.Client client)
    {
        _client = client;
    }

    public Task<Session?> SignUp(string email, string senha)
    {
        return _client.Auth.SignUp(email, senha);
    }

    public Task<Session?> SignIn(string email, string senha)
    {
        return _client.Auth.SignIn(email, senha);
    }
}
