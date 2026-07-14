using MemoriaAfro.Core.Enum;

namespace MemoriaAfro.Core.Models;

/// <summary>
/// Upload enviado por um visitante ao Acervo Colaborativo
/// (foto antiga, documento ou relato de família).
/// </summary>
public class UploadMemoria
{
    public int Id { get; set; }

    /// <summary>Preenchido quando o autor envia sem estar logado ("Seu Nome" no formulário).</summary>
    public string NomeAutor { get; set; } = string.Empty;

    public string Titulo { get; set; } = string.Empty;

    public string Descricao { get; set; } = string.Empty;

    /// <summary>Bytes da imagem, gravados diretamente no PostgreSQL (bytea).</summary>
    public byte[]? ImagemBytes { get; set; }

    public string? ImagemContentType { get; set; }

    public DateTime DataEnvio { get; set; } = DateTime.UtcNow;

    public StatusUpload Status { get; set; } = StatusUpload.Pendente;

    /// <summary>Preenchido automaticamente quando o autor está autenticado.</summary>
    public string? UsuarioId { get; set; }

    public Usuario? Usuario { get; set; }
}
