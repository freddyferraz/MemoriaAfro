using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Services;
using MemoriaAfro.Infra.Data;
using MemoriaAfro.Infra.Repositories;
using MemoriaAfro.Infra.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MemoriaAfro.Infra;

/// <summary>
/// Ponto único de registro de tudo que pertence à camada de infraestrutura:
/// DbContext (Postgres), Identity, repositórios e serviços concretos.
/// Chamado a partir do Program.cs do projeto Web via AddInfrastructure(...).
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services
            .AddIdentity<ApplicationIdentityUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

        services.AddHttpClient<IGoogleMapsLinkService, GoogleMapsLinkService>(client =>
        {
            client.Timeout = TimeSpan.FromSeconds(10);
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MemoriaAfroApi/1.0)");
        })
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { AllowAutoRedirect = false });

        // Repositórios
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IUploadMemoriaRepository, UploadMemoriaRepository>();
        services.AddScoped<IHistoricoPontosRepository, HistoricoPontosRepository>();
        services.AddScoped<IPontoInteresseRepository, PontoInteresseRepository>();
        services.AddScoped<IDocumentarioRepository, DocumentarioRepository>();
        services.AddScoped<IArquivoImersivoRepository, ArquivoImersivoRepository>();
        services.AddScoped<ITutorialOficinaRepository, TutorialOficinaRepository>();

        // Serviços
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IPontosService, PontosService>();

        return services;
    }
}
