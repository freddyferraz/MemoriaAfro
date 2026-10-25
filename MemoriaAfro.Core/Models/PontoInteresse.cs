using MemoriaAfro.Core.Enum;

namespace MemoriaAfro.Core.Models;

/// <summary>
/// Ponto de interesse exibido no Mapa Histórico georreferenciado.
/// </summary>
public class PontoInteresse
{
    public int Id { get; set; }

    public string Nome { get; set; } = string.Empty;

    public CategoriaPontoInteresse Categoria { get; set; }

    public string Descricao { get; set; } = string.Empty;

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    /// <summary>Se falso, o ponto fica oculto do mapa público (ex.: em revisão).</summary>
    public bool AtivoNoMapa { get; set; } = true;

    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;
    public string UrlImagem { get; set; }

    public ICollection<ArquivoImersivo> ArquivosImersivos { get; set; } = new List<ArquivoImersivo>();
}
