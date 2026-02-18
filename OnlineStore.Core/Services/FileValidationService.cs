using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace OnlineStore.Core.Services;

public class FileValidationService
{
    private readonly FileStorageOptions _options;
    
    // Разрешенные типы файлов
    private static readonly Dictionary<string, List<byte[]>> AllowedFileTypes = new()
    {
        // Изображения
        { "image/jpeg", new List<byte[]> { new byte[] { 0xFF, 0xD8, 0xFF } } },
        { "image/png", new List<byte[]> { new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A } } },
        { "image/gif", new List<byte[]> { new byte[] { 0x47, 0x49, 0x46, 0x38 } } },
        { "image/webp", new List<byte[]> { new byte[] { 0x52, 0x49, 0x46, 0x46 }, new byte[] { 0x57, 0x45, 0x42, 0x50 } } },
        
        // Документы
        { "application/pdf", new List<byte[]> { new byte[] { 0x25, 0x50, 0x44, 0x46 } } },
        { "application/msword", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } },
        { "application/vnd.ms-excel", new List<byte[]> { new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 } } },
        { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", new List<byte[]> { new byte[] { 0x50, 0x4B, 0x03, 0x04 } } }
    };
    
    // Разрешенные расширения файлов
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".gif", ".webp",
        ".pdf", ".doc", ".docx", ".xls", ".xlsx"
    };
    
    public FileValidationService(IOptions<FileStorageOptions> options)
    {
        _options = options.Value;
    }
    
    /// <summary>
    /// Валидация одного файла
    /// </summary>
    /// <param name="file">Файл для валидации</param>
    /// <returns>Результат валидации</returns>
    public async Task<(bool IsValid, string ErrorMessage)> ValidateFileAsync(IFormFile file)
    {
        // Проверка на null
        if (file == null)
        {
            return (false, "Файл не предоставлен");
        }
        
        // Проверка размера файла
        if (file.Length > _options.MaxFileSizeBytes)
        {
            return (false, $"Размер файла превышает максимальный допустимый ({_options.MaxFileSizeBytes / (1024 * 1024)}MB)");
        }
        
        if (file.Length == 0)
        {
            return (false, "Файл пуст");
        }
        
        // Проверка расширения файла
        var extension = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(extension) || !AllowedExtensions.Contains(extension))
        {
            return (false, "Тип файла не поддерживается");
        }
        
        // Проверка на path traversal
        if (IsPathTraversal(file.FileName))
        {
            return (false, "Недопустимое имя файла");
        }
        
        // Проверка magic bytes
        var (isValid, errorMessage) = await ValidateMagicBytesAsync(file);
        if (!isValid)
        {
            return (false, errorMessage);
        }
        
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Валидация множественных файлов
    /// </summary>
    /// <param name="files">Список файлов для валидации</param>
    /// <returns>Результат валидации</returns>
    public async Task<(bool IsValid, string ErrorMessage)> ValidateMultipleFilesAsync(List<IFormFile> files)
    {
        // Проверка на null
        if (files == null || files.Count == 0)
        {
            return (false, "Файлы не предоставлены");
        }
        
        // Проверка максимального количества файлов
        if (files.Count > _options.MaxFilesPerUpload)
        {
            return (false, $"Превышено максимальное количество файлов ({_options.MaxFilesPerUpload})");
        }
        
        // Проверка общего размера файлов
        var totalSize = files.Sum(f => f.Length);
        if (totalSize > _options.MaxTotalUploadBytes)
        {
            return (false, $"Общий размер файлов превышает максимальный допустимый ({_options.MaxTotalUploadBytes / (1024 * 1024)}MB)");
        }
        
        // Валидация каждого файла
        foreach (var file in files)
        {
            var (isValid, errorMessage) = await ValidateFileAsync(file);
            if (!isValid)
            {
                return (false, $"Файл '{file.FileName}': {errorMessage}");
            }
        }
        
        return (true, string.Empty);
    }
    
    /// <summary>
    /// Проверка magic bytes файла
    /// </summary>
    /// <param name="file">Файл для проверки</param>
    /// <returns>Результат проверки</returns>
    private async Task<(bool IsValid, string ErrorMessage)> ValidateMagicBytesAsync(IFormFile file)
    {
        try
        {
            using var stream = file.OpenReadStream();
            using var reader = new BinaryReader(stream);
            
            // Читаем первые 8 байт для проверки
            var buffer = new byte[8];
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            
            // Сбрасываем позицию потока для дальнейшего использования
            stream.Position = 0;
            
            if (bytesRead == 0)
            {
                return (false, "Файл пуст");
            }
            
            // Проверяем magic bytes для каждого разрешенного типа
            foreach (var (contentType, magicBytesList) in AllowedFileTypes)
            {
                foreach (var magicBytes in magicBytesList)
                {
                    if (bytesRead >= magicBytes.Length)
                    {
                        var fileHeader = buffer.Take(magicBytes.Length).ToArray();
                        if (fileHeader.SequenceEqual(magicBytes))
                        {
                            // Дополнительная проверка для документов, которые могут иметь одинаковые заголовки
                            if (contentType == "application/msword" || contentType == "application/vnd.ms-excel")
                            {
                                // Проверяем расширение файла
                                var extension = Path.GetExtension(file.FileName).ToLower();
                                if ((contentType == "application/msword" && (extension == ".doc" || extension == ".dot")) ||
                                    (contentType == "application/vnd.ms-excel" && (extension == ".xls" || extension == ".xlt")))
                                {
                                    return (true, string.Empty);
                                }
                            }
                            else if (contentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" ||
                                     contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                            {
                                // Для DOCX/XLSX проверяем расширение
                                var extension = Path.GetExtension(file.FileName).ToLower();
                                if ((contentType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document" && extension == ".docx") ||
                                    (contentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" && extension == ".xlsx"))
                                {
                                    return (true, string.Empty);
                                }
                            }
                            else
                            {
                                return (true, string.Empty);
                            }
                        }
                    }
                }
            }
            
            return (false, "Тип файла не поддерживается (проверка magic bytes)");
        }
        catch (Exception ex)
        {
            return (false, $"Ошибка при проверке файла: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Проверка на path traversal
    /// </summary>
    /// <param name="fileName">Имя файла</param>
    /// <returns>True если обнаружен path traversal</returns>
    private bool IsPathTraversal(string fileName)
    {
        // Проверка на недопустимые символы и последовательности
        var invalidChars = Path.GetInvalidFileNameChars();
        if (fileName.Any(c => invalidChars.Contains(c)))
        {
            return true;
        }
        
        // Проверка на path traversal последовательности
        if (fileName.Contains("..") || fileName.Contains('/') || fileName.Contains('\\'))
        {
            return true;
        }
        
        // Проверка длины имени файла
        if (fileName.Length > 255)
        {
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Генерация безопасного имени файла
    /// </summary>
    /// <param name="originalFileName">Оригинальное имя файла</param>
    /// <returns>Безопасное имя файла</returns>
    public string GenerateSafeFileName(string originalFileName)
    {
        var extension = Path.GetExtension(originalFileName);
        var safeFileName = $"{Guid.NewGuid()}{extension}";
        return safeFileName;
    }
    
    /// <summary>
    /// Создание структуры директорий для хранения файлов
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Путь к директории</returns>
    public string GenerateStoragePath(Guid userId)
    {
        var now = DateTime.UtcNow;
        return Path.Combine("users", userId.ToString(), now.Year.ToString(), now.Month.ToString("00"), now.Day.ToString("00"));
    }
}