namespace OnlineStore.Core.DTOs;

public class ProductImageCreateDto
{
    public Microsoft.AspNetCore.Http.IFormFile? File { get; set; }
    public int? FileId { get; set; }
    public bool IsMain { get; set; } = false;
    public int Order { get; set; } = 0;
}

public class ProductImageUpdateDto
{
    public bool? IsMain { get; set; }
    public int? Order { get; set; }
}

public class ProductImageResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int FileId { get; set; }
    public bool IsMain { get; set; }
    public int Order { get; set; }
    
    // Данные файла
    public FileMetadataDTO File { get; set; } = null!;
    
    // URL для превью и полного изображения
    public string ThumbnailUrl { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
}