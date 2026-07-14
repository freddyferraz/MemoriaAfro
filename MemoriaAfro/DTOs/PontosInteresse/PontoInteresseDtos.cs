using System.ComponentModel.DataAnnotations;
using MemoriaAfro.Core.Enum;

namespace MemoriaAfro.DTOs.PontosInteresse;

public class PontoInteresseDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public CategoriaPontoInteresse Categoria { get; set; }
    public string Descricao { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class PontoInteresseCreateDto
{
    [Required, MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public CategoriaPontoInteresse Categoria { get; set; }

    [Required, MaxLength(2000)]
    public string Descricao { get; set; } = string.Empty;

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }
}
