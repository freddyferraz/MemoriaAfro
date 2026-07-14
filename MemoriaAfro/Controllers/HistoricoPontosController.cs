using System.Security.Claims;
using MemoriaAfro.Core.Enum;
using MemoriaAfro.Core.Services;
using MemoriaAfro.DTOs.HistoricoPontos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MemoriaAfro.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HistoricoPontosController : ControllerBase
{
    private readonly IPontosService _pontosService;

    public HistoricoPontosController(IPontosService pontosService)
    {
        _pontosService = pontosService;
    }

    /// <summary>Saldo atual de pontos do usuário autenticado.</summary>
    [HttpGet("saldo")]
    public async Task<ActionResult<SaldoPontosDto>> ObterSaldo()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized();
        }

        var saldo = await _pontosService.ObterSaldoAsync(usuarioId);
        return Ok(new SaldoPontosDto { Saldo = saldo });
    }

    /// <summary>Extrato completo de movimentações (créditos e débitos) do usuário autenticado.</summary>
    [HttpGet("extrato")]
    public async Task<ActionResult<IEnumerable<HistoricoPontosDto>>> ObterExtrato()
    {
        var usuarioId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(usuarioId))
        {
            return Unauthorized();
        }

        var movimentacoes = await _pontosService.ObterExtratoAsync(usuarioId);

        return Ok(movimentacoes.Select(m => new HistoricoPontosDto
        {
            Id = m.Id,
            TipoMovimentacao = m.TipoMovimentacao,
            Quantidade = m.Quantidade,
            Descricao = m.Descricao,
            DataMovimentacao = m.DataMovimentacao
        }));
    }
}
