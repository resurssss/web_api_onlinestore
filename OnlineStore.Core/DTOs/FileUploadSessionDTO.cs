namespace OnlineStore.Core.DTOs;

public class FileUploadSessionDTO
{
    public string UploadId { get; set; } = string.Empty;
    public int UploadedBy { get; set; }
    public string FileName { get; set; } = string.Empty;
    public int TotalChunks { get; set; }
    public int UploadedChunks { get; set; }
    public long TotalSize { get; set; }
    public bool IsCompleted { get; set; }
}
