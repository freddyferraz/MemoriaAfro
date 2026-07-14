using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.DTOs.Tutoriais;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemoriaAfro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TutoriaisController : ControllerBase
{
    private readonly ITutorialOficinaRepository _repository;

    public TutoriaisController(ITutorialOficinaRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<TutorialOficinaDto>>> Listar()
    {
        var tutoriais = await _repository.ListarAsync();
        return Ok(tutoriais.OrderByDescending(t => t.CriadoEm).Select(MapearParaDto));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<TutorialOficinaDto>> ObterPorId(int id)
    {
        var tutorial = await _repository.ObterPorIdAsync(id);
        return tutorial is null ? NotFound() : Ok(MapearParaDto(tutorial));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<TutorialOficinaDto>> Criar(TutorialOficinaCreateDto dto)
    {
        var tutorial = new TutorialOficina
        {
            Titulo = dto.Titulo,
            ThumbnailUrl = dto.ThumbnailUrl,
            VideoUrl = dto.VideoUrl,
            DuracaoMinutos = dto.DuracaoMinutos
        };

        await _repository.AdicionarAsync(tutorial);
        await _repository.SalvarAlteracoesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = tutorial.Id }, MapearParaDto(tutorial));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Remover(int id)
    {
        var tutorial = await _repository.ObterPorIdAsync(id);
        if (tutorial is null)
        {
            return NotFound();
        }

        _repository.Remover(tutorial);
        await _repository.SalvarAlteracoesAsync();
        return NoContent();
    }

    private static TutorialOficinaDto MapearParaDto(TutorialOficina t) => new()
    {
        Id = t.Id,
        Titulo = t.Titulo,
        ThumbnailUrl = t.ThumbnailUrl,
        VideoUrl = t.VideoUrl,
        DuracaoMinutos = t.DuracaoMinutos
    };
}
