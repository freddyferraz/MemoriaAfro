using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MemoriaAfro.DTOs.Uploads;

/// <summary>
/// Recebido via multipart/form-data, correspondendo ao formulário
/// "Envie sua Contribuição" (Seu Nome, Título, História, Arquivo).
/// </summary>
public class UploadCreateDto
{
    [Required, MaxLength(150)]
    public string NomeAutor { get; set; } = string.Empty;

    [Required, MaxLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required, MaxLength(4000)]
    public string Descricao { get; set; } = string.Empty;

    /// <summary>Foto anexada ("Clique ou arraste a imagem aqui"). Opcional.</summary>
    public IFormFile? Arquivo { get; set; }
}
