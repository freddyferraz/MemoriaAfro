namespace MemoriaAfro.Core.Models;

/// <summary>
/// Gravação de oficina (ex.: "Oficina de Bonecas Abayomi") listada
/// na Biblioteca de Tutoriais.
/// </summary>
public class TutorialOficina
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public string? ThumbnailUrl { get; set; }

    public string VideoUrl { get; set; } = string.Empty;

    public int DuracaoMinutos { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
