using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.DTOs.Documentarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemoriaAfro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DocumentariosController : ControllerBase
{
    private readonly IDocumentarioRepository _repository;

    public DocumentariosController(IDocumentarioRepository repository)
    {
        _repository = repository;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<DocumentarioDto>>> Listar()
    {
        var documentarios = await _repository.ListarAsync();
        return Ok(documentarios.OrderByDescending(d => d.CriadoEm).Select(MapearParaDto));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<DocumentarioDto>> ObterPorId(int id)
    {
        var documentario = await _repository.ObterPorIdAsync(id);
        return documentario is null ? NotFound() : Ok(MapearParaDto(documentario));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<DocumentarioDto>> Criar(DocumentarioCreateDto dto)
    {
        var documentario = new Documentario
        {
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            VideoUrl = dto.VideoUrl,
            ThumbnailUrl = dto.ThumbnailUrl,
            DuracaoMinutos = dto.DuracaoMinutos
        };

        await _repository.AdicionarAsync(documentario);
        await _repository.SalvarAlteracoesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = documentario.Id }, MapearParaDto(documentario));
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Atualizar(int id, DocumentarioCreateDto dto)
    {
        var documentario = await _repository.ObterPorIdAsync(id);
        if (documentario is null)
        {
            return NotFound();
        }

        documentario.Titulo = dto.Titulo;
        documentario.Descricao = dto.Descricao;
        documentario.VideoUrl = dto.VideoUrl;
        documentario.ThumbnailUrl = dto.ThumbnailUrl;
        documentario.DuracaoMinutos = dto.DuracaoMinutos;

        _repository.Atualizar(documentario);
        await _repository.SalvarAlteracoesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Remover(int id)
    {
        var documentario = await _repository.ObterPorIdAsync(id);
        if (documentario is null)
        {
            return NotFound();
        }

        _repository.Remover(documentario);
        await _repository.SalvarAlteracoesAsync();
        return NoContent();
    }

    private static DocumentarioDto MapearParaDto(Documentario d) => new()
    {
        Id = d.Id,
        Titulo = d.Titulo,
        Descricao = d.Descricao,
        VideoUrl = d.VideoUrl,
        ThumbnailUrl = d.ThumbnailUrl,
        DuracaoMinutos = d.DuracaoMinutos
    };
}
