using System.ComponentModel.DataAnnotations;

namespace MemoriaAfro.DTOs.Documentarios;

public class DocumentarioDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;
    public string VideoUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public int DuracaoMinutos { get; set; }
}

public class DocumentarioCreateDto
{
    [Required, MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Descricao { get; set; } = string.Empty;

    [Required, Url, MaxLength(500)]
    public string VideoUrl { get; set; } = string.Empty;

    [Url, MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    [Range(1, 600)]
    public int DuracaoMinutos { get; set; }
}
