using MemoriaAfro.Core.Enum;

namespace MemoriaAfro.Core.Models;

/// <summary>
/// Registro em vídeo acessível via QR Code espalhados pela cidade
/// (ex.: "Roda de Jongo da Serrinha").
/// </summary>
public class ArquivoImersivo
{
    public int Id { get; set; }

    public string Titulo { get; set; } = string.Empty;

    public TipoRegistroImersivo TipoRegistro { get; set; } = TipoRegistroImersivo.Multicamera;

    public string VideoUrl { get; set; } = string.Empty;

    /// <summary>Ponto histórico físico onde o QR Code está afixado (opcional).</summary>
    public int? PontoInteresseId { get; set; }

    public PontoInteresse? PontoInteresse { get; set; }

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
}
