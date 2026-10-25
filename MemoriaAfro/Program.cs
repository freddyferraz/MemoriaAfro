using System.Text;
using MemoriaAfro.Infra;
using MemoriaAfro.Infra.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// ---------- Render: converte postgres://... em connection string do Npgsql ----------
// Precisa vir ANTES do AddInfrastructure, que lê "DefaultConnection".
// Localmente (Host=...;Database=...) este bloco não faz nada.
var rawConn = builder.Configuration.GetConnectionString("DefaultConnection");
if (!string.IsNullOrWhiteSpace(rawConn) &&
    (rawConn.StartsWith("postgres://") || rawConn.StartsWith("postgresql://")))
{
    var uri = new Uri(rawConn);
    var userInfo = uri.UserInfo.Split(':', 2);

    builder.Configuration["ConnectionStrings:DefaultConnection"] =
        new Npgsql.NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port > 0 ? uri.Port : 5432,
            Database = uri.AbsolutePath.TrimStart('/'),
            Username = Uri.UnescapeDataString(userInfo[0]),
            Password = Uri.UnescapeDataString(userInfo[1])
        }.ToString();
}

// Registra DbContext (Postgres), Identity, repositórios e serviços do Infra.
builder.Services.AddInfrastructure(builder.Configuration);

// ---------- Autenticação JWT ----------
var jwtSettings = builder.Configuration.GetSection("Jwt");
var jwtKey = jwtSettings["Key"] ?? throw new InvalidOperationException("Jwt:Key não configurada em appsettings.json.");
builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// ---------- CORS (ajuste a origem para o domínio real do front-end) ----------
const string corsPolicy = "FrontendPolicy";
builder.Services.AddCors(options =>
{
    options.AddPolicy(corsPolicy, policy =>
        policy.WithOrigins(builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? Array.Empty<string>())
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ---------- Controllers e Swagger ----------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Memória Afro API", Version = "v1" });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Informe: Bearer {seu token JWT}",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// ---------- Render: porta definida pela plataforma ----------
// Só é aplicada quando a variável PORT existe (no computador vale o launchSettings.json).
var port = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(port))
{
    app.Urls.Add($"http://0.0.0.0:{port}");
}

// ---------- Migrations + Seed de Roles e Administradores ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    // Executa o seeder consolidado de roles e da lista de administradores
    await ApplicationDbInitializer.SeedUsersAndRolesAsync(scope.ServiceProvider);
}

// Swagger: em Development sempre; em produção só se Swagger__Enabled=true.
if (app.Environment.IsDevelopment() || app.Configuration.GetValue<bool>("Swagger:Enabled"))
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// No Render o HTTPS é terminado pelo proxy, então o redirect só faz sentido localmente.
if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

// Serve arquivos estáticos de wwwroot/ (ex.: index.html do protótipo).
// UseDefaultFiles precisa vir ANTES de UseStaticFiles para que "/" resolva para index.html.
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors(corsPolicy);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();