using MemoriaAfro.Core.Enum;

namespace MemoriaAfro.DTOs.Uploads;

public class UploadDto
{
    public int Id { get; set; }
    public string NomeAutor { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string Descricao { get; set; } = string.Empty;

    /// <summary>URL para baixar a imagem (endpoint dedicado), evitando trafegar bytes no JSON.</summary>
    public string? ImagemUrl { get; set; }

    public DateTime DataEnvio { get; set; }
    public StatusUpload Status { get; set; }
}
