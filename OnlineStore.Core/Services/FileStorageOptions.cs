namespace OnlineStore.Core.Services;

public class FileStorageOptions
{
    public string RootPath { get; set; } = "uploads";
    public long MaxFileSizeBytes { get; set; } = 100 * 1024 * 1024; // 100 MB
    public int MaxFilesPerUpload { get; set; } = 10;
    public long MaxTotalUploadBytes { get; set; } = 500 * 1024 * 1024; // 500 MB
    public long StreamingThresholdBytes { get; set; } = 10 * 1024 * 1024; // 10 MB
}