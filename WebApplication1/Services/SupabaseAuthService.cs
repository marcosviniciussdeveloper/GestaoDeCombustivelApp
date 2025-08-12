using Supabase;
using Supabase.Gotrue;
using WebApplication1.Repositories.Interfaces;
using Client = Supabase.Client;

public class SupabaseAuthService : ISupabaseAuthService
{
    private readonly string _url;
    private readonly string _anon;

    public SupabaseAuthService(IConfiguration cfg)
    {
        _url  = cfg["Supabase:Url"]  ?? throw new InvalidOperationException("Supabase:Url is not configured.");
        _anon = cfg["Supabase:AnonKey"] ?? throw new InvalidOperationException("Supabase:AnonKey is not configured.");
    }

    private Client CreateAnonClient() =>
        new Client(_url, _anon, new SupabaseOptions { AutoRefreshToken = false });

    public Task<Session?> SignUp(string email, string senha)
        => CreateAnonClient().Auth.SignUp(email, senha);

    public Task<Session?> SignIn(string email, string senha)
        => CreateAnonClient().Auth.SignIn(email, senha);
}
