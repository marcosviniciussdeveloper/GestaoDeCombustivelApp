using Meucombustivel.Repositories;
using Meucombustivel.Repositories.Interfaces;
using Meucombustivel.Services;
using Meucombustivel.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using WebApplication1.Repositories;
using WebApplication1.Repositories.Interfaces;
using WebApplication1.Services;
using WebApplication1.Services.Interface;
using WebApplication1.ServicesAuth;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// ========================
// Configurações Supabase
// ========================
var supabaseUrl       = builder.Configuration["Supabase:Url"]             ?? throw new InvalidOperationException("Supabase:Url is not configured.");
var supabaseAnonKey   = builder.Configuration["Supabase:AnonKey"]         ?? throw new InvalidOperationException("Supabase:AnonKey is not configured.");
var supabaseServiceKey= builder.Configuration["Supabase:ServiceRoleKey"]  ?? throw new InvalidOperationException("Supabase:ServiceRoleKey is not configured.");

builder.Services.AddSingleton(_ =>
    new Supabase.Client(
        supabaseUrl,
        supabaseServiceKey,
        new Supabase.SupabaseOptions
        {
            AutoRefreshToken = false
        })
);

// Bind para opções usadas no DashboardService (Url/ApiKey)
builder.Services.Configure<WebApplication1.Services.SupabaseSettings>(
    builder.Configuration.GetSection("Supabase")
);

// ========================
// JWT
// ========================
var jwtKey      = builder.Configuration["Jwt:Key"]      ?? throw new InvalidOperationException("Jwt:Key is not configured.");
var jwtIssuer   = builder.Configuration["Jwt:Issuer"]   ?? throw new InvalidOperationException("Jwt:Issuer is not configured.");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Jwt:Audience is not configured.");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = jwtIssuer,
            ValidAudience            = jwtAudience,
            IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

// ========================
// AutoMapper
// ========================
builder.Services.AddAutoMapper(Assembly.GetExecutingAssembly());

// ========================
// Injeção de dependências
// ========================
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IMotoristaRepository, MotoristaRepository>();
builder.Services.AddScoped<IAbastecimentoRepository, AbastecimentoRepository>();
builder.Services.AddScoped<IEmpresaRepository, EmpresaRepository>();
builder.Services.AddScoped<IManutencaoRepository, ManutencoesRepository>();
builder.Services.AddScoped<IVeiculoRepository, VeiculoRepository>();
builder.Services.AddScoped<IEmpresaMotoristaRepository, EmpresaMotoristaRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IEmpresaMotoristaService, EmpresaMotoristaService>();
builder.Services.AddScoped<ISupabaseAuthService, SupabaseAuthService>();
builder.Services.AddScoped<IMotoristaService, MotoristaService>();
builder.Services.AddScoped<IAbastecimentoService, AbastecimentoService>();
builder.Services.AddScoped<IManutencaoService, ManutencaoService>();
builder.Services.AddScoped<IVeiculoService, VeiculoService>();
builder.Services.AddScoped<IEmpresaService, EmpresaService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddHttpClient<IDashBoardService, DashboardService>((sp, client) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();

    var baseUrl   = cfg["Supabase:Url"]            ?? throw new InvalidOperationException("Supabase:Url is not configured.");
    var anonKey   = cfg["Supabase:AnonKey"]        ?? throw new InvalidOperationException("Supabase:AnonKey is not configured.");
    var serviceKey= cfg["Supabase:ServiceRoleKey"] ?? throw new InvalidOperationException("Supabase:ServiceRoleKey is not configured.");

    if (!baseUrl.EndsWith("/")) baseUrl += "/";
    client.BaseAddress = new Uri($"{baseUrl}rest/v1/");

    client.DefaultRequestHeaders.Clear();
    client.DefaultRequestHeaders.Add("apikey", anonKey);
    client.DefaultRequestHeaders.Authorization =
        new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", serviceKey);
});

// ========================
// Swagger
// ========================
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Meu Combustível API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<Meucombustivel.Middlewares.ExceptionHandlerMiddleware>();
    app.UseHsts();
}

if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Meu Combustível API V1");
        c.RoutePrefix = string.Empty;
        c.EnableTryItOutByDefault();
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseCors("AllowAll");
app.UseAuthorization();

app.MapControllers();

app.Run();
