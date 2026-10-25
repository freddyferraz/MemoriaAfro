using System.ComponentModel.DataAnnotations;
using System.Globalization;
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

    /// <summary>URL da imagem representativa do ponto histórico.</summary>
    public string? UrlImagem { get; set; }

    /// <summary>Abre o Google Maps com a rota da localização atual do usuário até este ponto.</summary>
    public string LinkRota =>
        string.Create(CultureInfo.InvariantCulture,
            $"https://www.google.com/maps/dir/?api=1&destination={Latitude},{Longitude}&travelmode=driving");
}

/// <summary>
/// Usado no POST e no PUT. Informe LinkGoogleMaps (as coordenadas são extraídas do link)
/// OU Latitude + Longitude manualmente.
/// </summary>
public class PontoInteresseCreateDto : IValidatableObject
{
    [Required, MaxLength(150)]
    public string Nome { get; set; } = string.Empty;

    [Required]
    [EnumDataType(typeof(CategoriaPontoInteresse))]
    public CategoriaPontoInteresse Categoria { get; set; }

    [Required, MaxLength(2000)]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Link copiado do Google Maps (completo ou encurtado, como https://maps.app.goo.gl/xxxx).</summary>
    [MaxLength(2000)]
    [Url(ErrorMessage = "Informe um link válido.")]
    public string? LinkGoogleMaps { get; set; }

    [Range(-90, 90)]
    public double? Latitude { get; set; }

    [Range(-180, 180)]
    public double? Longitude { get; set; }

    /// <summary>URL opcional para exibir a imagem do local.</summary>
    [MaxLength(2000)]
    [Url(ErrorMessage = "Informe uma URL válida para a imagem.")]
    public string? UrlImagem { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        // Não permite enviar só uma das duas coordenadas.
        if (Latitude.HasValue != Longitude.HasValue)
        {
            yield return new ValidationResult(
                "Informe Latitude e Longitude juntas, ou use o LinkGoogleMaps.",
                new[] { nameof(Latitude), nameof(Longitude) });
        }
    }
}