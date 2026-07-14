using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Core.Services;
using MemoriaAfro.DTOs.Auth;
using MemoriaAfro.Infra.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace MemoriaAfro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationIdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly ITokenService _tokenService;

    public AuthController(
        UserManager<ApplicationIdentityUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IUsuarioRepository usuarioRepository,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _usuarioRepository = usuarioRepository;
        _tokenService = tokenService;
    }

    /// <summary>Cria uma nova conta (Identity + registro de domínio) com a role padrão "Cliente".</summary>
    [HttpPost("registrar")]
    public async Task<ActionResult<AuthResponseDto>> Registrar(RegisterDto dto)
    {
        var identityExistente = await _userManager.FindByEmailAsync(dto.Email);
        if (identityExistente is not null)
        {
            return Conflict("Já existe uma conta com este e-mail.");
        }

        var identityUser = new ApplicationIdentityUser
        {
            UserName = dto.Email,
            Email = dto.Email
        };

        var resultado = await _userManager.CreateAsync(identityUser, dto.Senha);
        if (!resultado.Succeeded)
        {
            return BadRequest(resultado.Errors.Select(e => e.Description));
        }

        const string roleCliente = "Cliente";
        if (!await _roleManager.RoleExistsAsync(roleCliente))
        {
            await _roleManager.CreateAsync(new IdentityRole(roleCliente));
        }
        await _userManager.AddToRoleAsync(identityUser, roleCliente);

        // Cria o registro de domínio (Core.Models.Usuario) compartilhando o mesmo Id.
        var usuario = new Usuario
        {
            Id = identityUser.Id,
            NomeCompleto = dto.NomeCompleto,
            Email = dto.Email
        };
        await _usuarioRepository.AdicionarAsync(usuario);
        await _usuarioRepository.SalvarAlteracoesAsync();

        return await MontarResposta(identityUser, usuario);
    }

    /// <summary>Autentica o usuário e retorna um JWT.</summary>
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto dto)
    {
        var identityUser = await _userManager.FindByEmailAsync(dto.Email);
        if (identityUser is null || !await _userManager.CheckPasswordAsync(identityUser, dto.Senha))
        {
            return Unauthorized("E-mail ou senha inválidos.");
        }

        var usuario = await _usuarioRepository.ObterPorIdAsync(identityUser.Id);
        if (usuario is null)
        {
            return Unauthorized("Cadastro incompleto. Contate o suporte.");
        }

        return await MontarResposta(identityUser, usuario);
    }

    private async Task<AuthResponseDto> MontarResposta(ApplicationIdentityUser identityUser, Usuario usuario)
    {
        var roles = await _userManager.GetRolesAsync(identityUser);
        var (token, expiraEm) = _tokenService.GerarToken(usuario, roles);

        return new AuthResponseDto
        {
            Token = token,
            ExpiraEm = expiraEm,
            Email = usuario.Email,
            NomeCompleto = usuario.NomeCompleto,
            Roles = roles
        };
    }
}
