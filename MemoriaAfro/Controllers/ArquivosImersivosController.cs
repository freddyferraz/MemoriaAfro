using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.DTOs.ArquivosImersivos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemoriaAfro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ArquivosImersivosController : ControllerBase
{
    private readonly IArquivoImersivoRepository _repository;

    public ArquivosImersivosController(IArquivoImersivoRepository repository)
    {
        _repository = repository;
    }

    /// <summary>Lista os registros do Arquivo Imersivo. Suporta filtrar por ponto histórico (QR Code).</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<ArquivoImersivoDto>>> Listar([FromQuery] int? pontoInteresseId)
    {
        var arquivos = pontoInteresseId.HasValue
            ? await _repository.ListarPorPontoInteresseAsync(pontoInteresseId.Value)
            : await _repository.ListarAsync();

        return Ok(arquivos.OrderByDescending(a => a.CriadoEm).Select(MapearParaDto));
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ArquivoImersivoDto>> ObterPorId(int id)
    {
        var arquivo = await _repository.ObterPorIdAsync(id);
        return arquivo is null ? NotFound() : Ok(MapearParaDto(arquivo));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ArquivoImersivoDto>> Criar(ArquivoImersivoCreateDto dto)
    {
        var arquivo = new ArquivoImersivo
        {
            Titulo = dto.Titulo,
            TipoRegistro = dto.TipoRegistro,
            VideoUrl = dto.VideoUrl,
            PontoInteresseId = dto.PontoInteresseId
        };

        await _repository.AdicionarAsync(arquivo);
        await _repository.SalvarAlteracoesAsync();

        return CreatedAtAction(nameof(ObterPorId), new { id = arquivo.Id }, MapearParaDto(arquivo));
    }

    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Remover(int id)
    {
        var arquivo = await _repository.ObterPorIdAsync(id);
        if (arquivo is null)
        {
            return NotFound();
        }

        _repository.Remover(arquivo);
        await _repository.SalvarAlteracoesAsync();
        return NoContent();
    }

    private static ArquivoImersivoDto MapearParaDto(ArquivoImersivo a) => new()
    {
        Id = a.Id,
        Titulo = a.Titulo,
        TipoRegistro = a.TipoRegistro,
        VideoUrl = a.VideoUrl,
        PontoInteresseId = a.PontoInteresseId
    };
}
