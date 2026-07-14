using System.Security.Claims;
using MemoriaAfro.Core.Enum;
using MemoriaAfro.Core.Interfaces;
using MemoriaAfro.Core.Models;
using MemoriaAfro.Core.Services;
using MemoriaAfro.DTOs.Uploads;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemoriaAfro.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadsController : ControllerBase
{
    private const long TamanhoMaximoArquivoBytes = 5 * 1024 * 1024; // 5 MB
    private static readonly string[] TiposPermitidos = { "image/jpeg", "image/png", "image/webp" };

    private readonly IUploadMemoriaRepository _uploadMemoriaRepository;
    private readonly IPontosService _pontosService;

    public UploadsController(IUploadMemoriaRepository uploadMemoriaRepository, IPontosService pontosService)
    {
        _uploadMemoriaRepository = uploadMemoriaRepository;
        _pontosService = pontosService;
    }

    /// <summary>Lista os uploads já aprovados ("Registros Recentes").</summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<UploadDto>>> Listar()
    {
        var uploads = await _uploadMemoriaRepository.ListarPorStatusAsync(StatusUpload.Aprovado);
        return Ok(uploads.Select(MapearParaDto));
    }

    /// <summary>Lista uploads pendentes de moderação. Restrito a Admin/Moderador.</summary>
    [HttpGet("pendentes")]
    [Authorize(Roles = "Admin,Moderador")]
    public async Task<ActionResult<IEnumerable<UploadDto>>> ListarPendentes()
    {
        var uploads = await _uploadMemoriaRepository.ListarPorStatusAsync(StatusUpload.Pendente);
        return Ok(uploads.Select(MapearParaDto));
    }

    /// <summary>Recebe um novo upload ("Enviar para o Acervo"). Aberto ao público.</summary>
    [HttpPost]
    [AllowAnonymous]
    [RequestSizeLimit(TamanhoMaximoArquivoBytes)]
    public async Task<ActionResult<UploadDto>> Enviar([FromForm] UploadCreateDto dto)
    {
        byte[]? imagemBytes = null;
        string? imagemContentType = null;

        if (dto.Arquivo is { Length: > 0 } arquivo)
        {
            if (arquivo.Length > TamanhoMaximoArquivoBytes)
            {
                return BadRequest("O arquivo excede o tamanho máximo de 5 MB.");
            }

            if (!TiposPermitidos.Contains(arquivo.ContentType))
            {
                return BadRequest("Formato de imagem não suportado. Envie JPEG, PNG ou WEBP.");
            }

            using var stream = new MemoryStream();
            await arquivo.CopyToAsync(stream);
            imagemBytes = stream.ToArray();
            imagemContentType = arquivo.ContentType;
        }

        var upload = new UploadMemoria
        {
            NomeAutor = dto.NomeAutor,
            Titulo = dto.Titulo,
            Descricao = dto.Descricao,
            ImagemBytes = imagemBytes,
            ImagemContentType = imagemContentType,
            Status = StatusUpload.Pendente,
            UsuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier)
        };

        await _uploadMemoriaRepository.AdicionarAsync(upload);
        await _uploadMemoriaRepository.SalvarAlteracoesAsync();

        return CreatedAtAction(nameof(ObterImagem), new { id = upload.Id }, MapearParaDto(upload));
    }

    /// <summary>Retorna os bytes da imagem armazenados no banco.</summary>
    [HttpGet("{id:int}/imagem")]
    [AllowAnonymous]
    public async Task<IActionResult> ObterImagem(int id)
    {
        var upload = await _uploadMemoriaRepository.ObterPorIdAsync(id);
        if (upload?.ImagemBytes is null)
        {
            return NotFound();
        }

        return File(upload.ImagemBytes, upload.ImagemContentType ?? "application/octet-stream");
    }

    /// <summary>
    /// Aprova ou rejeita um upload pendente. Restrito a Admin/Moderador.
    /// Ao aprovar, credita pontos de fidelidade ao autor (se autenticado).
    /// </summary>
    [HttpPatch("{id:int}/moderar")]
    [Authorize(Roles = "Admin,Moderador")]
    public async Task<IActionResult> Moderar(int id, UploadModerarDto dto)
    {
        var upload = await _uploadMemoriaRepository.ObterPorIdAsync(id);
        if (upload is null)
        {
            return NotFound();
        }

        upload.Status = dto.NovoStatus;
        _uploadMemoriaRepository.Atualizar(upload);
        await _uploadMemoriaRepository.SalvarAlteracoesAsync();

        if (dto.NovoStatus == StatusUpload.Aprovado)
        {
            await _pontosService.CreditarPorUploadAprovadoAsync(upload);
        }

        return NoContent();
    }

    private static UploadDto MapearParaDto(UploadMemoria u) => new()
    {
        Id = u.Id,
        NomeAutor = u.NomeAutor,
        Titulo = u.Titulo,
        Descricao = u.Descricao,
        ImagemUrl = u.ImagemBytes is not null ? $"/api/uploads/{u.Id}/imagem" : null,
        DataEnvio = u.DataEnvio,
        Status = u.Status
    };
}
