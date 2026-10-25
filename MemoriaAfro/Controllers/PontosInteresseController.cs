using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Core.Services;
using MemoriaAfro.Core.Struct;
using MemoriaAfro.DTOs.PontosInteresse;
using MemoriaAfro.Infra.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemoriaAfro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PontosInteresseController : ControllerBase
{
    private readonly IPontoInteresseRepository _repository;
    private readonly IGoogleMapsLinkService _mapsLinkService;

    public PontosInteresseController(
        IPontoInteresseRepository repository,
        IGoogleMapsLinkService mapsLinkService)
    {
        _repository = repository;
        _mapsLinkService = mapsLinkService;
    }

    /// <summary>Lista os pontos ativos no mapa. Suporta busca por nome via ?termo=.</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<PontoInteresseDto>>> Listar([FromQuery] string? termo)
    {
        var pontos = string.IsNullOrWhiteSpace(termo)
            ? await _repository.ListarAsync()
            : await _repository.BuscarPorNomeAsync(termo);

        return Ok(pontos.Where(p => p.AtivoNoMapa).OrderBy(p => p.Nome).Select(MapearParaDto));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<PontoInteresseDto>> ObterPorId(int id)
    {
        var ponto = await _repository.ObterPorIdAsync(id);
        if (ponto is null || !ponto.AtivoNoMapa)
        {
            return NotFound();
        }

        return Ok(MapearParaDto(ponto));
    }

    /// <summary>
    /// Cria um novo ponto no mapa. Restrito a administradores.
    /// Informe LinkGoogleMaps (as coordenadas são extraídas do link)
    /// OU Latitude + Longitude manualmente.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PontoInteresseDto>> Criar(
        PontoInteresseCreateDto dto,
        CancellationToken ct)
    {
        var (coordenadas, erro) = await ResolverCoordenadasAsync(dto, ct);
        if (coordenadas is null)
        {
            return BadRequest(new { erro });
        }

        var ponto = new PontoInteresse
        {
            Nome = dto.Nome,
            Categoria = dto.Categoria,
            Descricao = dto.Descricao,
            UrlImagem = dto.UrlImagem, // <-- Atribuição da imagem adicionada aqui
            Latitude = coordenadas.Value.Latitude,
            Longitude = coordenadas.Value.Longitude
        };

        await _repository.AdicionarAsync(ponto);
        await _repository.SalvarAlteracoesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = ponto.Id }, MapearParaDto(ponto));
    }

    /// <summary>
    /// Atualiza um ponto existente. Restrito a administradores.
    /// Se nenhum link/coordenada for enviado, a localização atual é mantida.
    /// </summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Atualizar(
        int id,
        PontoInteresseCreateDto dto,
        CancellationToken ct)
    {
        var ponto = await _repository.ObterPorIdAsync(id);
        if (ponto is null)
        {
            return NotFound();
        }

        var enviouLocalizacao = !string.IsNullOrWhiteSpace(dto.LinkGoogleMaps)
                                || (dto.Latitude is not null && dto.Longitude is not null);

        if (enviouLocalizacao)
        {
            var (coordenadas, erro) = await ResolverCoordenadasAsync(dto, ct);
            if (coordenadas is null)
            {
                return BadRequest(new { erro });
            }

            ponto.Latitude = coordenadas.Value.Latitude;
            ponto.Longitude = coordenadas.Value.Longitude;
        }

        ponto.Nome = dto.Nome;
        ponto.Categoria = dto.Categoria;
        ponto.Descricao = dto.Descricao;
        ponto.UrlImagem = dto.UrlImagem; // <-- Atualização da imagem adicionada aqui

        _repository.Atualizar(ponto);
        await _repository.SalvarAlteracoesAsync();
        return NoContent();
    }

    /// <summary>Remove (desativa) um ponto do mapa. Restrito a administradores.</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Remover(int id)
    {
        var ponto = await _repository.ObterPorIdAsync(id);
        if (ponto is null)
        {
            return NotFound();
        }

        ponto.AtivoNoMapa = false;
        _repository.Atualizar(ponto);
        await _repository.SalvarAlteracoesAsync();
        return NoContent();
    }

    /// <summary>
    /// Prioridade: link do Google Maps; se não houver link, usa Latitude/Longitude informadas.
    /// </summary>
    private async Task<(Coordenadas? Coordenadas, string? Erro)> ResolverCoordenadasAsync(
        PontoInteresseCreateDto dto,
        CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(dto.LinkGoogleMaps))
        {
            var doLink = await _mapsLinkService.ExtrairCoordenadasAsync(dto.LinkGoogleMaps, ct);
            return doLink is null
                ? (null, "Não foi possível extrair a localização do link. Abra o local no Google Maps, " +
                         "clique no pin e copie o link da barra de endereço (ou use Compartilhar > Copiar link).")
                : (doLink, null);
        }

        if (dto.Latitude is { } lat && dto.Longitude is { } lng)
        {
            return (new Coordenadas(lat, lng), null);
        }

        return (null, "Informe o LinkGoogleMaps ou então Latitude e Longitude.");
    }

    private static PontoInteresseDto MapearParaDto(PontoInteresse p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Categoria = p.Categoria,
        Descricao = p.Descricao,
        Latitude = p.Latitude,
        Longitude = p.Longitude,
        UrlImagem = p.UrlImagem // <-- Mapeamento de retorno para o DTO adicionado aqui
    };
}