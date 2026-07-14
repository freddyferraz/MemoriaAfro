using System.ComponentModel.DataAnnotations;

namespace MemoriaAfro.DTOs.Tutoriais;

public class TutorialOficinaDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public int DuracaoMinutos { get; set; }
}

public class TutorialOficinaCreateDto
{
    [Required, MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Url, MaxLength(500)]
    public string? ThumbnailUrl { get; set; }

    [Required, Url, MaxLength(500)]
    public string VideoUrl { get; set; } = string.Empty;

    [Range(1, 600)]
    public int DuracaoMinutos { get; set; }
}
