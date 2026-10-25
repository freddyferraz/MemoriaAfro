using MemoriaAfro.Core.Models;
using MemoriaAfro.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Globalization;
using System.Text;

namespace MemoriaAfro.Infra.Data;

public static class ApplicationDbInitializer
{
    public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationIdentityUser>>();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        // 1. Criar a Role de Administrador, caso não exista
        string adminRole = "Admin";
        if (!await roleManager.RoleExistsAsync(adminRole))
        {
            await roleManager.CreateAsync(new IdentityRole(adminRole));
        }

        // Lista de nomes completos dos administradores
        var nomesAdmins = new List<string>
        {
            "Alex Sandro dos Santos de Souza",
            "Amanda Francieli do Nascimento",
            "Frederico Hadan Silva Ferraz Rodrigues",
            "Joelson Ferreira dos Santos",
            "Juliana Campos Degenario Ribeiro",
            "Mateus Bruno Santos de Sousa",
            "Rodrigo Toledo de Jesus",
            "Rodrigo Yokoyama"
        };

        // 2. Iterar sobre a lista e cadastrar os administradores no Identity E na tabela Usuarios
        foreach (var nomeCompleto in nomesAdmins)
        {
            string email = GerarEmailPadrao(nomeCompleto);

            var usuarioIdentity = await userManager.FindByEmailAsync(email);

            if (usuarioIdentity == null)
            {
                usuarioIdentity = new ApplicationIdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(usuarioIdentity, "Admin@TechFlow2026!");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(usuarioIdentity, adminRole);
                }
            }
            else
            {
                // Garante que a role Admin está associada caso o usuário já exista
                if (!await userManager.IsInRoleAsync(usuarioIdentity, adminRole))
                {
                    await userManager.AddToRoleAsync(usuarioIdentity, adminRole);
                }
            }

            // 3. Sincronizar / Garantir cadastro na tabela de domínio 'Usuarios'
            var usuarioDominioExiste = await dbContext.Usuarios.AnyAsync(u => u.Id == usuarioIdentity.Id);
            if (!usuarioDominioExiste)
            {
                var novoUsuarioDominio = new Usuario
                {
                    Id = usuarioIdentity.Id, // Mesma chave primária do Identity
                    NomeCompleto = nomeCompleto,
                    Email = email,
                    CriadoEm = DateTime.UtcNow
                };

                dbContext.Usuarios.Add(novoUsuarioDominio);
                await dbContext.SaveChangesAsync();
            }
        }
    }

    private static string GerarEmailPadrao(string nomeCompleto)
    {
        var partes = nomeCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

        string primeiroNome = RemoverAcentos(partes.First().ToLower());
        string ultimoNome = RemoverAcentos(partes.Last().ToLower());

        return $"{primeiroNome}.{ultimoNome}@techflow.com.br";
    }

    private static string RemoverAcentos(string texto)
    {
        var textoNormalizado = texto.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in textoNormalizado)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }
}