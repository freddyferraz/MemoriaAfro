namespace MemoriaAfro.Core.Models;

/// <summary>
/// Documentário oficial exibido na página "Acervo &amp; Oficinas".
/// </summary>
public class Documentario
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    public string VideoUrl { get; set; } = string.Empty;

    public string? ThumbnailUrl { get; set; }

    public int DuracaoMinutos { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
