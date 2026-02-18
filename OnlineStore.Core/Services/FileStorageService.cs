using Microsoft.EntityFrameworkCore;
using OnlineStore.Core.Models;
using System.Security.Cryptography;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
// using SixLabors.ImageSharp.Formats;

namespace OnlineStore.Core.Services;

public class FileStorageService
{
    private readonly string _storagePath;
    
    public FileStorageService(string storagePath = "uploads")
    {
        _storagePath = storagePath;
        // Создаем директорию для хранения файлов, если она не существует
        if (!Directory.Exists(_storagePath))
        {
            Directory.CreateDirectory(_storagePath);
        }
    }
    
    /// <summary>
    /// Сохраняет файл на диск и возвращает метаданные
    /// </summary>
    /// <param name="file">Файл для сохранения</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="isPublic">Публичный доступ</param>
    /// <param name="expiresAt">Время истечения</param>
    /// <returns>Метаданные файла</returns>
    public async Task<FileMetadata> SaveFileAsync(
        Microsoft.AspNetCore.Http.IFormFile file,
        int userId,
        bool isPublic = false,
        DateTime? expiresAt = null)
    {
        // Генерируем безопасное имя файла
        var safeFileName = GenerateSafeFileName(file.FileName);
        
        // Создаем структуру директорий
        var storagePath = GenerateStoragePath(userId);
        var fullPath = Path.Combine(_storagePath, storagePath);
        
        // Создаем директории, если они не существуют
        if (!Directory.Exists(fullPath))
        {
            Directory.CreateDirectory(fullPath);
        }
        
        // Полный путь к файлу
        var filePath = Path.Combine(fullPath, safeFileName);
        
        // Сохраняем файл на диск
        using (var stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }
        
        // Вычисляем хеш файла
        var hash = await CalculateFileHashAsync(filePath);
        
        // Проверяем наличие файла с таким же хешем
        var existingFile = await FindFileByHashAsync(hash, userId);
        if (existingFile != null)
        {
            // Если файл уже существует, удаляем только что сохраненный файл
            File.Delete(filePath);
            
            // Возвращаем существующий файл
            return existingFile;
        }
        
        // Создаем метаданные файла
        var metadata = new FileMetadata
        {
            FileName = safeFileName,
            OriginalFileName = file.FileName,
            ContentType = file.ContentType,
            Size = file.Length,
            UploadedBy = userId,
            UploadedAt = DateTime.UtcNow,
            Path = Path.Combine(storagePath, safeFileName),
            Hash = hash,
            IsPublic = isPublic,
            ExpiresAt = expiresAt,
            DownloadCount = 0
        };
        
        // Если файл является изображением, обрабатываем его
        if (IsImageFile(file.ContentType))
        {
            await ProcessImageFileAsync(filePath, metadata);
        }
        
        return metadata;
    }
    
    /// <summary>
    /// Обработка изображения: оптимизация, извлечение метаданных, создание thumbnails
    /// </summary>
    /// <param name="filePath">Путь к файлу изображения</param>
    /// <param name="metadata">Метаданные файла</param>
    private async Task ProcessImageFileAsync(string filePath, FileMetadata metadata)
    {
        try
        {
            using var image = await Image.LoadAsync(filePath);
            
            // Извлекаем EXIF данные
            var exifProfile = image.Metadata.ExifProfile;
            if (exifProfile != null)
            {
                // Извлекаем дату съемки
                if (exifProfile.TryGetValue(ExifTag.DateTime, out var dateTaken) && dateTaken.Value != null)
                {
                    metadata.DateTaken = dateTaken.Value.ToString();
                }
                else if (exifProfile.TryGetValue(ExifTag.DateTimeOriginal, out var dateTakenOriginal) && dateTakenOriginal.Value != null)
                {
                    metadata.DateTaken = dateTakenOriginal.Value.ToString();
                }
                
                // Извлекаем модель камеры
                if (exifProfile.TryGetValue(ExifTag.Model, out var cameraModel) && cameraModel.Value != null)
                {
                    metadata.CameraModel = cameraModel.Value.ToString();
                }
                
                // Извлекаем GPS координаты
                if (exifProfile.TryGetValue(ExifTag.GPSLatitude, out var gpsLat) && gpsLat.Value != null &&
                    exifProfile.TryGetValue(ExifTag.GPSLongitude, out var gpsLon) && gpsLon.Value != null)
                {
                    metadata.Location = $"{string.Join(",", gpsLat.Value)}, {string.Join(",", gpsLon.Value)}";
                }
                
                // Извлекаем ориентацию
                if (exifProfile.TryGetValue(ExifTag.Orientation, out var orientation))
                {
                    metadata.Orientation = (int)orientation.Value;
                }
            }
            
            // Сохраняем размеры изображения
            metadata.Width = image.Width;
            metadata.Height = image.Height;
            
            // Оптимизируем изображение
            await OptimizeImageAsync(image, filePath, metadata.ContentType);
            
            // Создаем thumbnails
            await GenerateThumbnailsAsync(image, filePath, metadata.ContentType);
            
            // Удаляем EXIF данные из оригинального файла, если он является изображением
            /*if (metadata.IsPublic)
            {
                metadata.DateTaken = null;
                metadata.CameraModel = null;
                metadata.Location = null;
                metadata.Orientation = null;
            }*/
        }
        catch (Exception)
        {
            // В случае ошибки обработки изображения, продолжаем работу без создания thumbnails
            // Логирование ошибки можно добавить при необходимости
        }
    }
    
    /// <summary>
    /// Оптимизация изображения: уменьшение качества JPEG, удаление EXIF данных
    /// </summary>
    /// <param name="image">Изображение</param>
    /// <param name="filePath">Путь к файлу</param>
    /// <param name="contentType">Тип контента</param>
    private async Task OptimizeImageAsync(Image image, string filePath, string contentType)
    {
        // Удаляем EXIF данные для приватности
        image.Metadata.ExifProfile = null;
        
        // Сохраняем оптимизированное изображение
        switch (contentType.ToLower())
        {
            case "image/jpeg":
            case "image/jpg":
                var jpegEncoder = new JpegEncoder
                {
                    Quality = 90, // Уменьшение качества JPEG до 90%
                    SkipMetadata = true // Удаление метаданных
                };
                await image.SaveAsync(filePath, jpegEncoder);
                break;
                
            case "image/png":
                var pngEncoder = new PngEncoder
                {
                    SkipMetadata = true // Удаление метаданных
                };
                await image.SaveAsync(filePath, pngEncoder);
                break;
                
            case "image/gif":
                var gifEncoder = new GifEncoder
                {
                    SkipMetadata = true // Удаление метаданных
                };
                await image.SaveAsync(filePath, gifEncoder);
                break;
                
            case "image/webp":
                var webpEncoder = new WebpEncoder
                {
                    Quality = 90, // Уменьшение качества WebP до 90%
                    SkipMetadata = true // Удаление метаданных
                };
                await image.SaveAsync(filePath, webpEncoder);
                break;
        }
    }
    
    /// <summary>
    /// Генерация thumbnails для изображения
    /// </summary>
    /// <param name="image">Изображение</param>
    /// <param name="originalFilePath">Путь к оригинальному файлу</param>
    /// <param name="contentType">Тип контента</param>
    public async Task GenerateThumbnailsAsync(Image image, string originalFilePath, string contentType)
    {
        var directory = Path.GetDirectoryName(originalFilePath);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalFilePath);
        var extension = Path.GetExtension(originalFilePath);
        
        // Определяем кодировщик по типу контента
        SixLabors.ImageSharp.Formats.IImageEncoder encoder = contentType.ToLower() switch
        {
            "image/jpeg" or "image/jpg" => new JpegEncoder { Quality = 85 },
            "image/png" => new PngEncoder(),
            "image/gif" => new GifEncoder(),
            "image/webp" => new WebpEncoder { Quality = 85 },
            _ => new JpegEncoder { Quality = 85 }
        };
        
        // Создаем small thumbnail (200x200)
        using var smallImage = image.Clone(ctx => ctx.Resize(new ResizeOptions
        {
            Size = new Size(200, 200),
            Mode = ResizeMode.Max // Fit - вписать в размер
        }));
        
        var smallThumbnailPath = Path.Combine(directory ?? string.Empty, $"{fileNameWithoutExtension}_small{extension}");
        await smallImage.SaveAsync(smallThumbnailPath, encoder);
        
        // Создаем medium thumbnail (800x600)
        using var mediumImage = image.Clone(ctx => ctx.Resize(new ResizeOptions
        {
            Size = new Size(800, 600),
            Mode = ResizeMode.Max // Fit - вписать в размер
        }));
        
        var mediumThumbnailPath = Path.Combine(directory, $"{fileNameWithoutExtension}_medium{extension}");
        await mediumImage.SaveAsync(mediumThumbnailPath, encoder);
    }
    
    /// <summary>
    /// Проверка, является ли файл изображением
    /// </summary>
    /// <param name="contentType">Тип контента</param>
    /// <returns>Признак изображения</returns>
    private bool IsImageFile(string contentType)
    {
        return contentType.StartsWith("image/");
    }
    
    /// <summary>
    /// Генерация безопасного имени файла
    /// </summary>
    /// <param name="originalFileName">Оригинальное имя файла</param>
    /// <returns>Безопасное имя файла</returns>
    private string GenerateSafeFileName(string originalFileName)
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
    private string GenerateStoragePath(int userId)
    {
        var now = DateTime.UtcNow;
        return Path.Combine("users", userId.ToString(), now.Year.ToString(), now.Month.ToString("00"), now.Day.ToString("00"));
    }
    
    /// <summary>
    /// Вычисление SHA256 хеша файла
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    /// <returns>Хеш файла</returns>
    private async Task<string> CalculateFileHashAsync(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = await sha256.ComputeHashAsync(stream);
        return Convert.ToBase64String(hash);
    }
    
    /// <summary>
    /// Получение полного пути к файлу
    /// </summary>
    /// <param name="filePath">Относительный путь к файлу</param>
    /// <returns>Полный путь к файлу</returns>
    public string GetFullFilePath(string filePath)
    {
        return Path.Combine(_storagePath, filePath);
    }
    
    /// <summary>
    /// Удаление файла с диска
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    public void DeleteFile(string filePath)
    {
        var fullFilePath = GetFullFilePath(filePath);
        if (File.Exists(fullFilePath))
        {
            File.Delete(fullFilePath);
        }
    }
    
    /// <summary>
    /// Получение пути к thumbnail
    /// </summary>
    /// <param name="originalPath">Путь к оригинальному файлу</param>
    /// <param name="size">Размер thumbnail (small, medium)</param>
    /// <returns>Путь к thumbnail</returns>
    public string GetThumbnailPath(string originalPath, string size)
    {
        var directory = Path.GetDirectoryName(originalPath);
        var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(originalPath);
        var extension = Path.GetExtension(originalPath);
        
        var thumbnailFileName = $"{fileNameWithoutExtension}_{size}{extension}";
        return Path.Combine(directory ?? string.Empty, thumbnailFileName);
    }
    
    /// <summary>
    /// Поиск файла по хешу
    /// </summary>
    /// <param name="hash">Хеш файла</param>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Метаданные файла или null, если файл не найден</returns>
    private async Task<FileMetadata?> FindFileByHashAsync(string hash, int userId)
    {
        // Получаем контекст базы данных через DI
        using var context = new OnlineStoreDbContextFactory().CreateDbContext(Array.Empty<string>());
        
        // Ищем файл с таким же хешем, принадлежащий тому же пользователю
        return await context.FileMetadata
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.Hash == hash && f.UploadedBy == userId);
    }
}