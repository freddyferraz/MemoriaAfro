using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.DTOs.PontosInteresse;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemoriaAfro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PontosInteresseController : ControllerBase
{
    private readonly IPontoInteresseRepository _repository;

    public PontosInteresseController(IPontoInteresseRepository repository)
    {
        _repository = repository;
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

    /// <summary>Cria um novo ponto no mapa. Restrito a administradores.</summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<PontoInteresseDto>> Criar(PontoInteresseCreateDto dto)
    {
        var ponto = new PontoInteresse
        {
            Nome = dto.Nome,
            Categoria = dto.Categoria,
            Descricao = dto.Descricao,
            Latitude = dto.Latitude,
            Longitude = dto.Longitude
        };

        await _repository.AdicionarAsync(ponto);
        await _repository.SalvarAlteracoesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = ponto.Id }, MapearParaDto(ponto));
    }

    /// <summary>Atualiza um ponto existente. Restrito a administradores.</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Atualizar(int id, PontoInteresseCreateDto dto)
    {
        var ponto = await _repository.ObterPorIdAsync(id);
        if (ponto is null)
        {
            return NotFound();
        }

        ponto.Nome = dto.Nome;
        ponto.Categoria = dto.Categoria;
        ponto.Descricao = dto.Descricao;
        ponto.Latitude = dto.Latitude;
        ponto.Longitude = dto.Longitude;

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

    private static PontoInteresseDto MapearParaDto(PontoInteresse p) => new()
    {
        Id = p.Id,
        Nome = p.Nome,
        Categoria = p.Categoria,
        Descricao = p.Descricao,
        Latitude = p.Latitude,
        Longitude = p.Longitude
    };
}
