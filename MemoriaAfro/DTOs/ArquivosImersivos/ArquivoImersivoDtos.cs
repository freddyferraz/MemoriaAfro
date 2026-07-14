using System.ComponentModel.DataAnnotations;
using MemoriaAfro.Core.Enum;

namespace MemoriaAfro.DTOs.ArquivosImersivos;

public class ArquivoImersivoDto
{
    public int Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public TipoRegistroImersivo TipoRegistro { get; set; }
    public string VideoUrl { get; set; } = string.Empty;
    public int? PontoInteresseId { get; set; }
}

public class ArquivoImersivoCreateDto
{
    [Required, MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required]
    public TipoRegistroImersivo TipoRegistro { get; set; } = TipoRegistroImersivo.Multicamera;

    [Required, Url, MaxLength(500)]
    public string VideoUrl { get; set; } = string.Empty;

    public int? PontoInteresseId { get; set; }
}
