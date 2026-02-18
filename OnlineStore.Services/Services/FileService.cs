using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OnlineStore.Core;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Services;
using OnlineStore.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;
using System.Collections.Concurrent;
using OnlineStore.Core.Models;

namespace OnlineStore.Services.Services;

public class FileService
{
    private readonly OnlineStoreDbContext _context;
    private readonly ILogger<FileService> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly FileStorageOptions _options;
    private readonly FileValidationService _validationService;
    private readonly IAuditService _auditService;
    private static readonly ConcurrentDictionary<string, FileUploadSession> _uploadSessions = new();
    
    public FileService(
        OnlineStoreDbContext context,
        ILogger<FileService> logger,
        IWebHostEnvironment environment,
        IOptions<FileStorageOptions> options,
        FileValidationService validationService,
        IAuditService auditService)
    {
        _context = context;
        _logger = logger;
        _environment = environment;
        _options = options.Value;
        _validationService = validationService;
        _auditService = auditService;
    }
    
    /// <summary>
    /// Загрузка одного файла
    /// </summary>
    /// <param name="file">Файл для загрузки</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="isPublic">Публичный доступ</param>
    /// <param name="expiresAt">Время истечения</param>
    /// <returns>Метаданные файла</returns>
    public async Task<FileMetadataDTO> UploadFileAsync(IFormFile file, int userId, bool isPublic = false, DateTime? expiresAt = null)
    {
        try
        {
            // Валидация файла
            var (isValid, errorMessage) = await _validationService.ValidateFileAsync(file);
            if (!isValid)
            {
                throw new ArgumentException(errorMessage);
            }
            
            // Проверка квоты пользователя
            await CheckUserQuotaAsync(userId, file.Length);
            
            // Генерация безопасного пути к файлу
            var safeFileName = _validationService.GenerateSafeFileName(file.FileName);
            var storagePath = _validationService.GenerateStoragePath(Guid.NewGuid());
            var relativePath = Path.Combine(storagePath, safeFileName);
            
            // Сохранение файла на диск
            var fullPath = Path.Combine(_environment.ContentRootPath, _options.RootPath, relativePath);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
            
            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            // Создание thumbnail'ов для изображений
            if (file.ContentType.StartsWith("image/"))
            {
                await CreateThumbnailsAsync(fullPath, relativePath, safeFileName);
            }
            
            // Получение информации об изображении
            int? width = null;
            int? height = null;
            if (file.ContentType.StartsWith("image/"))
            {
                using (var image = await Image.LoadAsync(fullPath))
                {
                    width = image.Width;
                    height = image.Height;
                }
            }
            
            // Создание записи в БД
            var fileMetadata = new FileMetadata
            {
                FileName = safeFileName,
                OriginalFileName = file.FileName,
                ContentType = file.ContentType,
                Size = file.Length,
                UploadedBy = userId,
                Path = relativePath,
                Hash = CalculateFileHash(fullPath),
                IsPublic = isPublic,
                ExpiresAt = expiresAt,
                Width = width,
                Height = height
            };
            
            _context.FileMetadata.Add(fileMetadata);
            await _context.SaveChangesAsync();
            
            // Логирование аудита
            await _auditService.LogEventAsync(
                SecurityEventType.UserUpdated,
                userId,
                null,
                null,
                null,
                true,
                new { Action = "FileUpload", FileName = file.FileName, FileId = fileMetadata.Id }
            );
            
            return MapToFileMetadataDTO(fileMetadata);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла {FileName}", file?.FileName);
            throw; // Перебрасываем исключение для обработки на уровне контроллера
        }
    }
    
    /// <summary>
    /// Создание thumbnail'ов для изображения
    /// </summary>
    /// <param name="originalPath">Путь к оригинальному файлу</param>
    /// <param name="relativePath">Относительный путь к файлу</param>
    /// <param name="fileName">Имя файла</param>
    private async Task CreateThumbnailsAsync(string originalPath, string relativePath, string fileName)
    {
        try
        {
            // Определяем пути к thumbnail'ам
            var thumbnailDir = Path.Combine(_environment.ContentRootPath, _options.RootPath, "thumbnails", Path.GetDirectoryName(relativePath) ?? "");
            Directory.CreateDirectory(thumbnailDir);
            
            // Создаем thumbnail разных размеров
            var sizes = new Dictionary<string, int>
            {
                { "small", 150 },
                { "medium", 300 },
                { "large", 600 }
            };
            
            using (var image = await Image.LoadAsync(originalPath))
            {
                foreach (var (sizeName, size) in sizes)
                {
                    try
                    {
                        // Создаем копию изображения для изменения размера
                        using (var clone = image.Clone(ctx => ctx.Resize(new ResizeOptions
                        {
                            Size = new Size(size, size),
                            Mode = ResizeMode.Max
                        })))
                        {
                            // Определяем путь к thumbnail'у
                            var thumbnailFileName = $"{Path.GetFileNameWithoutExtension(fileName)}_{sizeName}.jpg";
                            var thumbnailPath = Path.Combine(thumbnailDir, thumbnailFileName);
                            
                            // Сохраняем thumbnail
                            await clone.SaveAsJpegAsync(thumbnailPath, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                            {
                                Quality = 80
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Ошибка при создании thumbnail {Size} для файла {FileName}", sizeName, fileName);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при создании thumbnail'ов для файла {FileName}", fileName);
        }
    }
    
    /// <summary>
    /// Загрузка нескольких файлов
    /// </summary>
    /// <param name="files">Список файлов для загрузки</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="isPublic">Публичный доступ</param>
    /// <param name="expiresAt">Время истечения</param>
    /// <returns>Список метаданных файлов</returns>
    public async Task<List<FileMetadataDTO>> UploadMultipleFilesAsync(
        List<IFormFile> files, int userId, bool isPublic = false, DateTime? expiresAt = null)
    {
        try
        {
            var results = new List<FileMetadataDTO>();
            
            foreach (var file in files)
            {
                try
                {
                    var result = await UploadFileAsync(file, userId, isPublic, expiresAt);
                    results.Add(result);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Ошибка при загрузке файла {FileName}", file.FileName);
                    // Продолжаем загрузку остальных файлов даже при ошибке одного
                    throw new InvalidOperationException($"Ошибка при загрузке файла {file.FileName}: {ex.Message}", ex);
                }
            }
            
            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке нескольких файлов");
            throw; // Перебрасываем исключение для обработки на уровне контроллера
        }
    }
    
    /// <summary>
    /// Получение списка файлов пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="isAdmin">Признак администратора</param>
    /// <param name="page">Номер страницы</param>
    /// <param name="pageSize">Размер страницы</param>
    /// <param name="contentType">Фильтр по типу контента</param>
    /// <param name="searchTerm">Поисковый запрос</param>
    /// <returns>Список метаданных файлов</returns>
    public async Task<PagedResultDto<FileMetadataDTO>> GetUserFilesAsync(
        int userId, bool isAdmin, int page, int pageSize, string? contentType, string? searchTerm)
    {
        try
        {
            IQueryable<FileMetadata> query = _context.FileMetadata;
            
            // Если не админ, показываем только свои файлы
            if (!isAdmin)
            {
                query = query.Where(f => f.UploadedBy == userId);
            }
            
            // Фильтрация по типу контента
            if (!string.IsNullOrEmpty(contentType))
            {
                query = query.Where(f => f.ContentType.Contains(contentType));
            }
            
            // Поиск по имени файла
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(f => f.OriginalFileName.Contains(searchTerm));
            }
            
            // Сортировка по дате загрузки (новые первыми)
            query = query.OrderByDescending(f => f.UploadedAt);
            
            // Пагинация
            var totalItems = await query.CountAsync();
            var files = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            
            var fileDtos = files.Select(MapToFileMetadataDTO).ToList();
            
            return new PagedResultDto<FileMetadataDTO>
            {
                Items = fileDtos,
                TotalCount = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка файлов пользователя {UserId}", userId);
            throw; // Перебрасываем исключение для обработки на уровне контроллера
        }
    }
    
    /// <summary>
    /// Получение метаданных файла
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="isAdmin">Признак администратора</param>
    /// <returns>Метаданные файла</returns>
    public async Task<FileMetadataDTO> GetFileMetadataAsync(int id, int? userId, bool isAdmin)
    {
        try
        {
            var file = await _context.FileMetadata.FindAsync(id);
            
            if (file == null)
            {
                throw new KeyNotFoundException($"Файл с ID {id} не найден");
            }
            
            // Проверка прав доступа
            if (!file.IsPublic && userId != file.UploadedBy && !isAdmin)
            {
                throw new UnauthorizedAccessException("Нет доступа к файлу");
            }
            
            // Проверка срока действия
            if (file.ExpiresAt.HasValue && file.ExpiresAt.Value < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Срок действия файла истек");
            }
            
            return MapToFileMetadataDTO(file);
        }
        catch (Exception ex) when (!(ex is KeyNotFoundException) && !(ex is UnauthorizedAccessException) && !(ex is InvalidOperationException))
        {
            _logger.LogError(ex, "Ошибка при получении метаданных файла {FileId}", id);
            throw; // Перебрасываем исключение для обработки на уровне контроллера
        }
    }
    
    /// <summary>
    /// Скачивание файла
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="isAdmin">Признак администратора</param>
    /// <returns>Путь к файлу и метаданные</returns>
    public async Task<(string filePath, FileMetadataDTO metadata)> DownloadFileAsync(int id, int? userId, bool isAdmin)
    {
        try
        {
            var file = await _context.FileMetadata.FindAsync(id);
            
            if (file == null)
            {
                throw new KeyNotFoundException($"Файл с ID {id} не найден");
            }
            
            // Проверка прав доступа - разрешаем доступ к публичным файлам без авторизации
            if (!file.IsPublic && userId != file.UploadedBy && !isAdmin)
            {
                throw new UnauthorizedAccessException("Нет доступа к файлу");
            }
            
            // Проверка срока действия
            if (file.ExpiresAt.HasValue && file.ExpiresAt.Value < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Срок действия файла истек");
            }
            
            var fullPath = Path.Combine(_environment.ContentRootPath, _options.RootPath, file.Path);
            
            // Увеличиваем счетчик скачиваний
            file.DownloadCount++;
            await _context.SaveChangesAsync();
            
            // Логирование аудита
            if (userId.HasValue)
            {
                await _auditService.LogEventAsync(
                    SecurityEventType.UserUpdated,
                    userId.Value,
                    null,
                    null,
                    null,
                    true,
                    new { Action = "FileDownload", FileName = file.OriginalFileName, FileId = file.Id }
                );
            }
            
            return (fullPath, MapToFileMetadataDTO(file));
        }
        catch (Exception ex) when (!(ex is KeyNotFoundException) && !(ex is UnauthorizedAccessException) && !(ex is InvalidOperationException))
        {
            _logger.LogError(ex, "Ошибка при скачивании файла {FileId}", id);
            throw; // Перебрасываем исключение для обработки на уровне контроллера
        }
    }
    
    /// <summary>
    /// Потоковая передача файла с поддержкой больших файлов
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="isAdmin">Признак администратора</param>
    /// <returns>Поток файла и метаданные</returns>
    public async Task<(Stream fileStream, FileMetadataDTO metadata)> StreamFileAsync(int id, int? userId, bool isAdmin)
{
    try
    {
        var file = await _context.FileMetadata.FindAsync(id);
        
        if (file == null)
        {
            throw new KeyNotFoundException($"Файл с ID {id} не найден");
        }
        
        // Проверка прав доступа - разрешаем доступ к публичным файлам без авторизации
        if (!file.IsPublic && !userId.HasValue)
        {
            throw new UnauthorizedAccessException("Файл доступен только авторизованным пользователям");
        }
        
        if (!file.IsPublic && userId != file.UploadedBy && !isAdmin)
        {
            throw new UnauthorizedAccessException("Нет доступа к файлу");
        }
        
        // Проверка срока действия
        if (file.ExpiresAt.HasValue && file.ExpiresAt.Value < DateTime.UtcNow)
        {
            throw new InvalidOperationException("Срок действия файла истек");
        }
        
        var fullPath = Path.Combine(_environment.ContentRootPath, _options.RootPath, file.Path);
        
        // Проверяем существование файла
        if (!File.Exists(fullPath))
        {
            throw new FileNotFoundException($"Файл не найден на диске: {fullPath}");
        }
        
        // КРИТИЧНО: Создаем поток с увеличенным буфером (80 KB оптимально для больших файлов)
        var fileStream = new FileStream(
            fullPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 81920, // 80 KB - оптимальный размер
            useAsync: true
        );
        
        // Увеличиваем счетчик скачиваний
        file.DownloadCount++;
        await _context.SaveChangesAsync();
        
        // Логирование аудита
        if (userId.HasValue)
        {
            await _auditService.LogEventAsync(
                SecurityEventType.UserUpdated,
                userId.Value,
                null,
                null,
                null,
                true,
                new
                {
                    Action = "FileStream",
                    FileName = file.OriginalFileName,
                    FileId = file.Id,
                    FileSize = new FileInfo(fullPath).Length
                }
            );
        }
        
        return (fileStream, MapToFileMetadataDTO(file));
    }
    catch (Exception ex) when (!(ex is KeyNotFoundException) &&
                                !(ex is UnauthorizedAccessException) &&
                                !(ex is InvalidOperationException) &&
                                !(ex is FileNotFoundException))
    {
        _logger.LogError(ex, "Ошибка при потоковой передаче файла {FileId}", id);
        throw;
    }
}
    
    /// <summary>
    /// Прямая отправка файла с оптимизацией для больших файлов
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="isAdmin">Признак администратора</param>
    /// <returns>Поток файла или массив байтов в зависимости от размера</returns>
    public async Task<object> DirectFileSendAsync(int id, int? userId, bool isAdmin)
    {
        try
        {
            var file = await _context.FileMetadata.FindAsync(id);
            
            if (file == null)
            {
                throw new KeyNotFoundException($"Файл с ID {id} не найден");
            }
            
            // Проверка прав доступа
            if (!file.IsPublic && userId != file.UploadedBy && !isAdmin)
            {
                throw new UnauthorizedAccessException("Нет доступа к файлу");
            }
            
            // Проверка срока действия
            if (file.ExpiresAt.HasValue && file.ExpiresAt.Value < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Срок действия файла истек");
            }
            
            var fullPath = Path.Combine(_environment.ContentRootPath, _options.RootPath, file.Path);
            
            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"Файл не найден на диске: {fullPath}");
            }
            
            var fileInfo = new FileInfo(fullPath);
            
            // Для больших файлов (больше 100MB) возвращаем FileStream для потоковой передачи
            if (fileInfo.Length > 100 * 1024 * 1024)
            {
                // Увеличиваем счетчик скачиваний
                file.DownloadCount++;
                await _context.SaveChangesAsync();
                
                // Логирование аудита
                if (userId.HasValue)
                {
                    await _auditService.LogEventAsync(
                        SecurityEventType.UserUpdated,
                        userId.Value,
                        null,
                        null,
                        null,
                        true,
                        new
                        {
                            Action = "DirectFileSend_Stream",
                            FileName = file.OriginalFileName,
                            FileId = file.Id,
                            FileSize = fileInfo.Length
                        }
                    );
                }
                
                var fileStream = new FileStream(
                    fullPath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read,
                    bufferSize: 81920, // 80 KB
                    useAsync: true
                );
                
                return new { Stream = fileStream, Metadata = MapToFileMetadataDTO(file) };
            }
            
            // Для небольших файлов читаем в память
            var fileBytes = await File.ReadAllBytesAsync(fullPath);
            
            // Увеличиваем счетчик скачиваний
            file.DownloadCount++;
            await _context.SaveChangesAsync();
            
            // Логирование аудита
            if (userId.HasValue)
            {
                await _auditService.LogEventAsync(
                    SecurityEventType.UserUpdated,
                    userId.Value,
                    null,
                    null,
                    null,
                    true,
                    new
                    {
                        Action = "DirectFileSend_Bytes",
                        FileName = file.OriginalFileName,
                        FileId = file.Id,
                        FileSize = fileBytes.Length
                    }
                );
            }
            
            return new { Bytes = fileBytes, Metadata = MapToFileMetadataDTO(file) };
        }
        catch (Exception ex) when (!(ex is KeyNotFoundException) &&
                                    !(ex is UnauthorizedAccessException) &&
                                    !(ex is InvalidOperationException) &&
                                    !(ex is FileNotFoundException))
        {
            _logger.LogError(ex, "Ошибка при прямой отправке файла {FileId}", id);
            throw;
        }
    }
    /// <summary>
    /// Получение thumbnail файла
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="isAdmin">Признак администратора</param>
    /// <param name="size">Размер thumbnail</param>
    /// <returns>Путь к thumbnail и метаданные</returns>
    public async Task<(string thumbnailPath, FileMetadataDTO metadata)> GetThumbnailAsync(int id, int? userId, bool isAdmin, string size)
    {
        try
        {
            var file = await _context.FileMetadata.FindAsync(id);
            
            if (file == null)
            {
                throw new KeyNotFoundException($"Файл с ID {id} не найден");
            }
            
            // Проверка, что файл является изображением
            if (!file.ContentType.StartsWith("image/"))
            {
                throw new InvalidOperationException("Файл не является изображением");
            }
            
            // Проверка прав доступа
            if (!file.IsPublic && userId != file.UploadedBy && !isAdmin)
            {
                throw new UnauthorizedAccessException("Нет доступа к файлу");
            }
            
            // Проверка срока действия
            if (file.ExpiresAt.HasValue && file.ExpiresAt.Value < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Срок действия файла истек");
            }
            
            // Определение пути к thumbnail
            var thumbnailFileName = $"{Path.GetFileNameWithoutExtension(file.FileName)}_{size}.jpg";
            var thumbnailPath = Path.Combine(
                _environment.ContentRootPath,
                _options.RootPath,
                "thumbnails",
                Path.GetDirectoryName(file.Path) ?? "",
                thumbnailFileName);
            
            // Проверка существования thumbnail
            if (!File.Exists(thumbnailPath))
            {
                // Если thumbnail не существует, попробуем создать его
                var originalPath = Path.Combine(_environment.ContentRootPath, _options.RootPath, file.Path);
                if (File.Exists(originalPath))
                {
                    await CreateThumbnailsAsync(originalPath, file.Path, file.FileName);
                    
                    // Проверим снова, существует ли thumbnail после создания
                    if (!File.Exists(thumbnailPath))
                    {
                        throw new FileNotFoundException("Не удалось создать thumbnail");
                    }
                }
                else
                {
                    throw new FileNotFoundException("Оригинальный файл не найден");
                }
            }
            
            return (thumbnailPath, MapToFileMetadataDTO(file));
        }
        catch (Exception ex) when (!(ex is KeyNotFoundException) && !(ex is UnauthorizedAccessException) && !(ex is InvalidOperationException) && !(ex is FileNotFoundException))
        {
            _logger.LogError(ex, "Ошибка при получении thumbnail файла {FileId}", id);
            throw; // Перебрасываем исключение для обработки на уровне контроллера
        }
    }
    
    /// <summary>
    /// Удаление файла
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <param name="userId">ID пользователя</param>
    /// <param name="isAdmin">Признак администратора</param>
    public async Task DeleteFileAsync(int id, int userId, bool isAdmin)
    {
        try
        {
            var file = await _context.FileMetadata.FindAsync(id);
            
            if (file == null)
            {
                throw new KeyNotFoundException($"Файл с ID {id} не найден");
            }
            
            // Проверка прав доступа
            if (userId != file.UploadedBy && !isAdmin)
            {
                throw new UnauthorizedAccessException("Нет прав на удаление файла");
            }
            
            // Soft delete вместо физического удаления
            file.IsDeleted = true;
            file.DeletedAt = DateTime.UtcNow;
            
            // Удаление файла с диска
            var fullPath = Path.Combine(_environment.ContentRootPath, _options.RootPath, file.Path);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
            
            // Удаление thumbnails
            if (file.Width.HasValue && file.Height.HasValue)
            {
                var thumbnailDir = Path.Combine(_environment.ContentRootPath, _options.RootPath, "thumbnails", Path.GetDirectoryName(file.Path) ?? "");
                var thumbnailFiles = Directory.GetFiles(thumbnailDir, $"{Path.GetFileNameWithoutExtension(file.FileName)}_*.*", SearchOption.AllDirectories);
                foreach (var thumbnailFile in thumbnailFiles)
                {
                    if (File.Exists(thumbnailFile))
                    {
                        File.Delete(thumbnailFile);
                    }
                }
            }
            
            // Обновление записи в БД (soft delete)
            _context.FileMetadata.Update(file);
            await _context.SaveChangesAsync();
            
            // Логирование аудита
            await _auditService.LogEventAsync(
                SecurityEventType.UserDeleted,
                userId,
                null,
                null,
                null,
                true,
                new { Action = "FileDelete", FileName = file.OriginalFileName, FileId = file.Id }
            );
        }
        catch (Exception ex) when (!(ex is KeyNotFoundException) && !(ex is UnauthorizedAccessException))
        {
            _logger.LogError(ex, "Ошибка при удалении файла {FileId}", id);
            throw; // Перебрасываем исключение для обработки на уровне контроллера
        }
    }
    
    /// <summary>
    /// Загрузка части файла (chunked upload)
    /// </summary>
    /// <param name="chunk">Часть файла</param>
    /// <param name="uploadId">ID загрузки</param>
    /// <param name="chunkIndex">Индекс части</param>
    /// <param name="totalChunks">Общее количество частей</param>
    /// <param name="fileName">Имя файла</param>
    /// <param name="userId">ID пользователя</param>
    /// <returns>Прогресс загрузки или метаданные файла</returns>
    public async Task<object> UploadChunkAsync(
        IFormFile chunk, string uploadId, int chunkIndex, int totalChunks, string fileName, int userId)
    {
        try
        {
            // Валидация части файла
            var (isValid, errorMessage) = await _validationService.ValidateFileAsync(chunk);
            if (!isValid)
            {
                throw new ArgumentException(errorMessage);
            }
            
            // Проверка квоты пользователя
            await CheckUserQuotaAsync(userId, chunk.Length);
            
            // Получение или создание сессии загрузки
            var session = _uploadSessions.GetOrAdd(uploadId, _ => new FileUploadSession
            {
                UploadId = uploadId,
                FileName = fileName,
                TotalChunks = totalChunks,
                UserId = userId,
                ReceivedChunks = new bool[totalChunks]
            });
            
            // Проверка правильности индекса части
            if (chunkIndex < 0 || chunkIndex >= totalChunks)
            {
                throw new ArgumentException("Неверный индекс части файла");
            }
            
            // Проверка, что часть еще не была загружена
            if (session.ReceivedChunks[chunkIndex])
            {
                throw new ArgumentException("Часть файла уже была загружена");
            }
            
            // Создание директории для временных файлов
            var tempDir = Path.Combine(_environment.ContentRootPath, _options.RootPath, "temp");
            Directory.CreateDirectory(tempDir);
            
            // Сохранение части файла
            var chunkPath = Path.Combine(tempDir, $"{uploadId}_{chunkIndex}");
            using (var stream = new FileStream(chunkPath, FileMode.Create))
            {
                await chunk.CopyToAsync(stream);
            }
            
            // Отмечаем часть как полученную
            session.ReceivedChunks[chunkIndex] = true;
            
            // Проверяем, все ли части получены
            if (session.ReceivedChunks.All(received => received))
            {
                // Собираем файл из частей
                var safeFileName = _validationService.GenerateSafeFileName(fileName);
                var storagePath = _validationService.GenerateStoragePath(Guid.NewGuid());
                var relativePath = Path.Combine(storagePath, safeFileName);
                var fullPath = Path.Combine(_environment.ContentRootPath, _options.RootPath, relativePath);
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                
                using (var outputStream = new FileStream(fullPath, FileMode.Create))
                {
                    for (int i = 0; i < totalChunks; i++)
                    {
                        var chunkFilePath = Path.Combine(tempDir, $"{uploadId}_{i}");
                        using (var inputStream = new FileStream(chunkFilePath, FileMode.Open))
                        {
                            await inputStream.CopyToAsync(outputStream);
                        }
                        
                        // Удаляем временную часть
                        File.Delete(chunkFilePath);
                    }
                }
                
                // Создание thumbnail'ов для изображений
                if (chunk.ContentType.StartsWith("image/"))
                {
                    await CreateThumbnailsAsync(fullPath, relativePath, safeFileName);
                }
                
                // Получение информации об изображении
                int? width = null;
                int? height = null;
                if (chunk.ContentType.StartsWith("image/"))
                {
                    using (var image = await Image.LoadAsync(fullPath))
                    {
                        width = image.Width;
                        height = image.Height;
                    }
                }
                
                // Создание записи в БД
                var fileMetadata = new FileMetadata
                {
                    FileName = safeFileName,
                    OriginalFileName = fileName,
                    ContentType = chunk.ContentType,
                    Size = new FileInfo(fullPath).Length,
                    UploadedBy = userId,
                    Path = relativePath,
                    Hash = CalculateFileHash(fullPath),
                    IsPublic = false,
                    Width = width,
                    Height = height
                };
                
                _context.FileMetadata.Add(fileMetadata);
                await _context.SaveChangesAsync();
                
                // Удаляем сессию
                _uploadSessions.TryRemove(uploadId, out _);
                
                // Логирование аудита
                await _auditService.LogEventAsync(
                    SecurityEventType.UserUpdated,
                    userId,
                    null,
                    null,
                    null,
                    true,
                    new { Action = "FileUploadChunked", FileName = fileName, FileId = fileMetadata.Id }
                );
                
                return MapToFileMetadataDTO(fileMetadata);
            }
            
            // Возвращаем прогресс загрузки
            var receivedCount = session.ReceivedChunks.Count(received => received);
            var progress = (double)receivedCount / totalChunks * 100;
            
            return new
            {
                UploadId = uploadId,
                Progress = Math.Round(progress, 2),
                ReceivedChunks = receivedCount,
                TotalChunks = totalChunks
            };
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            _logger.LogError(ex, "Ошибка при загрузке части файла {UploadId}, часть {ChunkIndex}", uploadId, chunkIndex);
            throw; // Перебрасываем исключение для обработки на уровне контроллера
        }
    }
    
    /// <summary>
    /// Получение прогресса загрузки
    /// </summary>
    /// <param name="uploadId">ID загрузки</param>
    /// <returns>Прогресс загрузки</returns>
    public async Task<object> GetUploadProgressAsync(string uploadId)
    {
        try
        {
            if (!_uploadSessions.TryGetValue(uploadId, out var session))
            {
                throw new ArgumentException("Сессия загрузки не найдена");
            }
            
            var receivedCount = session.ReceivedChunks.Count(received => received);
            var progress = (double)receivedCount / session.TotalChunks * 100;
            
            return new
            {
                UploadId = uploadId,
                Progress = Math.Round(progress, 2),
                ReceivedChunks = receivedCount,
                TotalChunks = session.TotalChunks,
                FileName = session.FileName
            };
        }
        catch (Exception ex) when (!(ex is ArgumentException))
        {
            _logger.LogError(ex, "Ошибка при получении прогресса загрузки {UploadId}", uploadId);
            throw; // Перебрасываем исключение для обработки на уровне контроллера
        }
    }
    
    /// <summary>
    /// Проверка квоты пользователя
    /// </summary>
    /// <param name="userId">ID пользователя</param>
    /// <param name="fileSize">Размер файла</param>
    private async Task CheckUserQuotaAsync(int userId, long fileSize)
    {
        var totalUserFilesSize = await _context.FileMetadata
            .Where(f => f.UploadedBy == userId)
            .SumAsync(f => (long?)f.Size) ?? 0;
        
        if (totalUserFilesSize + fileSize > _options.MaxTotalUploadBytes)
        {
            throw new InvalidOperationException($"Превышена квота пользователя. Максимум {_options.MaxTotalUploadBytes / (1024 * 1024)} МБ");
        }
    }
    
    /// <summary>
    /// Вычисление хэша файла
    /// </summary>
    /// <param name="filePath">Путь к файлу</param>
    /// <returns>Хэш файла</returns>
    private string CalculateFileHash(string filePath)
    {
        using (var stream = File.OpenRead(filePath))
        {
            using (var sha256 = SHA256.Create())
            {
                var hash = sha256.ComputeHash(stream);
                return Convert.ToBase64String(hash);
            }
        }
    }
    
    /// <summary>
    /// Маппинг FileMetadata в FileMetadataDTO
    /// </summary>
    /// <param name="fileMetadata">Метаданные файла</param>
    /// <returns>DTO метаданных файла</returns>
    private FileMetadataDTO MapToFileMetadataDTO(FileMetadata fileMetadata)
    {
        return new FileMetadataDTO
        {
            Id = fileMetadata.Id,
            FileName = fileMetadata.FileName,
            OriginalFileName = fileMetadata.OriginalFileName,
            ContentType = fileMetadata.ContentType,
            Size = fileMetadata.Size,
            UploadedBy = fileMetadata.UploadedBy,
            UploadedAt = fileMetadata.UploadedAt,
            Url = $"/api/files/{fileMetadata.Id}",
            IsPublic = fileMetadata.IsPublic,
            ExpiresAt = fileMetadata.ExpiresAt,
            DownloadCount = fileMetadata.DownloadCount,
            Hash = fileMetadata.Hash,
            Width = fileMetadata.Width,
            Height = fileMetadata.Height,
            DateTaken = fileMetadata.DateTaken,
            CameraModel = fileMetadata.CameraModel,
            Location = fileMetadata.Location,
            Orientation = fileMetadata.Orientation
        };
    }
}

/// <summary>
/// Сессия загрузки файла по частям
/// </summary>
public class FileUploadSession
{
    public string UploadId { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public int TotalChunks { get; set; }
    public int UserId { get; set; }
    public bool[] ReceivedChunks { get; set; } = Array.Empty<bool>();
}