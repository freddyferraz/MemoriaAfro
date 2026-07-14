using System.ComponentModel.DataAnnotations;
using MemoriaAfro.Core.Enum;

namespace MemoriaAfro.DTOs.Uploads;

public class UploadModerarDto
{
    [Required]
    public StatusUpload NovoStatus { get; set; }
}
