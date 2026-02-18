using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using OnlineStore.Services.Services;
using OnlineStore.Core.DTOs;
using OnlineStore.Core.Services;
using System.Security.Claims;
using AutoMapper;
using System.Security.Cryptography;
using OnlineStore.Core.Models;

namespace OnlineStore.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FilesController : ControllerBase
{
    private readonly FileService _fileService;
    private readonly ILogger<FilesController> _logger;
    private readonly IMapper _mapper;
    private readonly IWebHostEnvironment _environment;
    private readonly FileStorageOptions _options;
    
    public FilesController(
        FileService fileService,
        ILogger<FilesController> logger,
        IMapper mapper,
        IWebHostEnvironment environment,
        IOptions<FileStorageOptions> options)
    {
        _fileService = fileService;
        _logger = logger;
        _mapper = mapper;
        _environment = environment;
        _options = options.Value;
    }
    
    /// <summary>
    /// Загрузка одного файла
    /// </summary>
    /// <param name="file">Файл для загрузки</param>
    /// <param name="isPublic">Публичный доступ</param>
    /// <param name="expiresAt">Время истечения</param>
    /// <returns>Метаданные файла</returns>
    [HttpPost("upload")]
    public async Task<ActionResult<FileMetadataDTO>> UploadFile(
        [FromForm] Microsoft.AspNetCore.Http.IFormFile file,
        [FromForm] bool isPublic = false,
        [FromForm] DateTime? expiresAt = null)
    {
        try
        {
            int userId = GetUserId(); // Для загрузки файлов требуется авторизация
            var result = await _fileService.UploadFileAsync(file, userId, isPublic, expiresAt);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файла");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
    
    /// <summary>
    /// Загрузка нескольких файлов
    /// </summary>
    /// <param name="files">Список файлов для загрузки</param>
    /// <param name="isPublic">Публичный доступ</param>
    /// <param name="expiresAt">Время истечения</param>
    /// <returns>Список метаданных файлов</returns>
    [HttpPost("upload-multiple")]
    public async Task<ActionResult<List<FileMetadataDTO>>> UploadMultipleFiles(
        [FromForm] List<Microsoft.AspNetCore.Http.IFormFile> files,
        [FromForm] bool isPublic = false,
        [FromForm] DateTime? expiresAt = null)
    {
        try
        {
            if (files == null || files.Count == 0)
            {
                return BadRequest("Файлы не предоставлены");
            }
            
            int userId = GetUserId(); // Для загрузки файлов требуется авторизация
            var result = await _fileService.UploadMultipleFilesAsync(files, userId, isPublic, expiresAt);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке файлов");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
    
    /// <summary>
    /// Загрузка больших файлов по частям (chunked upload)
    /// </summary>
    /// <param name="chunk">Часть файла</param>
    /// <param name="uploadId">Уникальный идентификатор загрузки</param>
    /// <param name="chunkIndex">Индекс текущей части (0-based)</param>
    /// <param name="totalChunks">Общее количество частей</param>
    /// <param name="fileName">Имя исходного файла</param>
    /// <returns>Прогресс загрузки или метаданные файла</returns>
    [HttpPost("upload/chunked")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(5L * 1024 * 1024 * 1024)] // 5 GB
    public async Task<ActionResult<object>> UploadChunked(
        IFormFile chunk,
        [FromForm] string? uploadId = null,
        [FromForm] int chunkIndex = 0,
        [FromForm] int totalChunks = 1,
        [FromForm] string? fileName = null)
    {
        try
        {
            int userId = GetUserId();
            
            // Валидация параметров
            if (chunk == null || chunk.Length == 0)
            {
                return BadRequest("Файл не предоставлен");
            }
            
            uploadId = uploadId ?? Guid.NewGuid().ToString();
            if (uploadId.Length > 100)
            {
                return BadRequest("Слишком длинный идентификатор загрузки");
            }
            
            if (chunkIndex < 0 || totalChunks <= 0 || chunkIndex >= totalChunks)
            {
                return BadRequest("Неверные параметры загрузки");
            }
            
            fileName = fileName ?? chunk.FileName;
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return BadRequest("Имя файла не может быть пустым");
            }
            
            var result = await _fileService.UploadChunkAsync(chunk, uploadId, chunkIndex, totalChunks, fileName, userId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при загрузке части файла");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
    
    
    /// <summary>
    /// Получение списка файлов пользователя
    /// </summary>
    /// <param name="page">Номер страницы</param>
    /// <param name="pageSize">Размер страницы</param>
    /// <param name="contentType">Фильтр по типу контента</param>
    /// <param name="searchTerm">Поисковый запрос</param>
    /// <returns>Список метаданных файлов</returns>
    [HttpGet]
    public async Task<ActionResult<PagedResultDto<FileMetadataDTO>>> GetFiles(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? contentType = null,
        [FromQuery] string? searchTerm = null)
    {
        try
        {
            int userId = GetUserId(); // Для получения списка файлов требуется авторизация
            var isAdmin = IsAdmin();
            var result = await _fileService.GetUserFilesAsync(userId, isAdmin, page, pageSize, contentType, searchTerm);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении списка файлов");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
    
    /// <summary>
    /// Получение метаданных файла
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <returns>Метаданные файла</returns>
    [HttpGet("{id}/info")]
    public async Task<ActionResult<FileMetadataDTO>> GetFileInfo(int id)
    {
        try
        {
            int? userId = null;
            bool isAdmin = false;
            
            // Проверяем, авторизован ли пользователь
            if (User.Identity.IsAuthenticated)
            {
                userId = GetUserId();
                isAdmin = IsAdmin();
            }
            
            var result = await _fileService.GetFileMetadataAsync(id, userId, isAdmin);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Файл не найден");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении метаданных файла");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
    
    /// <summary>
    /// Скачивание файла
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <returns>Файл</returns>
    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> DownloadFile(int id)
    {
        try
        {
            int? userId = null;
            bool isAdmin = false;
            
            // Проверяем, авторизован ли пользователь
            if (User.Identity.IsAuthenticated)
            {
                userId = GetUserId();
                isAdmin = IsAdmin();
            }
            
            var (filePath, metadata) = await _fileService.DownloadFileAsync(id, userId, isAdmin);
            
            // Получаем абсолютный путь к файлу
            var fullFilePath = Path.GetFullPath(filePath);
            
            if (!System.IO.File.Exists(fullFilePath))
            {
                return NotFound("Файл не найден на диске");
            }
            
            // Определяем Content-Disposition (inline для изображений, attachment для остальных)
            var contentDisposition = metadata.ContentType.StartsWith("image/") ? "inline" : "attachment";
            
            // Создаем заголовок ETag
            var etag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue($"\"{metadata.Hash}\"");
            
            // Добавляем заголовки кэширования для изображений
            if (metadata.ContentType.StartsWith("image/"))
            {
                Response.Headers.CacheControl = "public, max-age=31536000";
            }
            
            return PhysicalFile(fullFilePath, metadata.ContentType, metadata.OriginalFileName,
                lastModified: metadata.UploadedAt,
                entityTag: etag,
                enableRangeProcessing: true);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Файл не найден");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("истек"))
        {
            return BadRequest("Срок действия файла истек");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при скачивании файла");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
    
    /// <summary>
    /// Получение thumbnail файла
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <param name="size">Размер thumbnail (small, medium)</param>
    /// <returns>Thumbnail файла</returns>
    [HttpGet("{id}/thumbnail")]
    [AllowAnonymous]
    public async Task<IActionResult> GetThumbnail(int id, [FromQuery] string size = "small")
    {
        try
        {
            int? userId = null;
            bool isAdmin = false;
            
            // Проверяем, авторизован ли пользователь
            if (User.Identity.IsAuthenticated)
            {
                userId = GetUserId();
                isAdmin = IsAdmin();
            }
            
            var (filePath, metadata) = await _fileService.GetThumbnailAsync(id, userId, isAdmin, size);
            
            // Получаем абсолютный путь к файлу
            var fullFilePath = Path.GetFullPath(filePath);
            
            if (!System.IO.File.Exists(fullFilePath))
            {
                return NotFound("Thumbnail не найден на диске");
            }
            
            // Создаем заголовок ETag
            var etag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue($"\"{metadata.Hash}\"");
            
            // Добавляем заголовки кэширования
            Response.Headers.CacheControl = "public, max-age=31536000";
            
            return PhysicalFile(fullFilePath, metadata.ContentType,
                lastModified: metadata.UploadedAt,
                entityTag: etag,
                enableRangeProcessing: true);
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Файл не найден");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("не является изображением"))
        {
            return BadRequest("Файл не является изображением");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("истек"))
        {
            return BadRequest("Срок действия файла истек");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при получении thumbnail");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
    
    /// <summary>
    /// Потоковая отдача файла для больших данных
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <returns>Файл в виде потока</returns>
    [HttpGet("{id}/stream")]
    [AllowAnonymous]
    public async Task<IActionResult> StreamFile(int id)
    {
        try
        {
            int? userId = null;
            bool isAdmin = false;
            
            // Проверяем, авторизован ли пользователь
            if (User.Identity.IsAuthenticated)
            {
                userId = GetUserId();
                isAdmin = IsAdmin();
            }
            
            // Используем потоковую передачу для больших файлов
            var (fileStream, metadata) = await _fileService.StreamFileAsync(id, userId, isAdmin);
            
            var contentType = metadata.ContentType ?? "application/octet-stream";
            
            return new FileStreamResult(fileStream, contentType)
            {
                EnableRangeProcessing = true,
                LastModified = metadata.UploadedAt,
                EntityTag = new Microsoft.Net.Http.Headers.EntityTagHeaderValue($"\"{metadata.Hash}\""),
                FileDownloadName = metadata.OriginalFileName
            };
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Файл не найден");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("истек"))
        {
            return BadRequest("Срок действия файла истек");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при потоковой передаче файла");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
    
    /// <summary>
    /// Получение файла для скачивания
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <returns>Файл для скачивания</returns>
    [HttpGet("download-link/{id}")]
    [Authorize]
    [DisableRequestSizeLimit]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Produces("application/octet-stream")]
    public async Task<IActionResult> GetDownloadLink(int id)
    {
        try
        {
            var userId = GetUserId();
            var isAdmin = IsAdmin();
            
            _logger.LogInformation("Скачивание файла ID={FileId}, User={UserId}, IsAdmin={IsAdmin}", id, userId, isAdmin);
            
            // Используем новый метод прямой отправки файлов
            var result = await _fileService.DirectFileSendAsync(id, userId, isAdmin);
            
            // Проверяем тип результата
            if (result is { } obj)
            {
                var resultDict = obj.GetType().GetProperty("Stream") != null ?
                    new Dictionary<string, object> {
                        { "Stream", obj.GetType().GetProperty("Stream")?.GetValue(obj) },
                        { "Metadata", obj.GetType().GetProperty("Metadata")?.GetValue(obj) }
                    } :
                    new Dictionary<string, object> {
                        { "Bytes", obj.GetType().GetProperty("Bytes")?.GetValue(obj) },
                        { "Metadata", obj.GetType().GetProperty("Metadata")?.GetValue(obj) }
                    };
                
                if (resultDict.ContainsKey("Stream") && resultDict["Stream"] != null)
                {
                    // Потоковая передача для больших файлов
                    var stream = (Stream)resultDict["Stream"];
                    var metadata = (FileMetadataDTO)resultDict["Metadata"];
                    var contentType = metadata.ContentType ?? "application/octet-stream";
                    
                    return new FileStreamResult(stream, contentType)
                    {
                        FileDownloadName = metadata.OriginalFileName,
                        EnableRangeProcessing = true
                    };
                }
                else if (resultDict.ContainsKey("Bytes") && resultDict["Bytes"] != null)
                {
                    // Отправка массива байтов для небольших файлов
                    var bytes = (byte[])resultDict["Bytes"];
                    var metadata = (FileMetadataDTO)resultDict["Metadata"];
                    var contentType = metadata.ContentType ?? "application/octet-stream";
                    
                    // Устанавливаем заголовки для скачивания файла
                    Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{metadata.OriginalFileName}\"");
                    Response.Headers.Add("X-Content-Type-Options", "nosniff");
                    Response.Headers.Add("Content-Length", bytes.Length.ToString());
                    
                    return File(bytes, contentType, metadata.OriginalFileName);
                }
            }
            
            return NotFound("Файл не найден");
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Файл не найден");
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("истек"))
        {
            return BadRequest("Срок действия файла истек");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при скачивании файла ID={FileId}", id);
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
    
    /// <summary>
    /// Получение ID текущего пользователя
    /// </summary>
    private int GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new UnauthorizedAccessException("Невозможно получить ID пользователя");
        }
        
        return userId;
    }
    
    /// <summary>
    /// Проверка, является ли пользователь администратором
    /// </summary>
    private bool IsAdmin()
    {
        return User.IsInRole("Admin") || User.IsInRole("Администратор");
    }
     
    /// <summary>
    /// Удаление файла
    /// </summary>
    /// <param name="id">ID файла</param>
    /// <returns>Результат операции</returns>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteFile(int id)
    {
        try
        {
            int userId = GetUserId();
            bool isAdmin = IsAdmin();
            
            await _fileService.DeleteFileAsync(id, userId, isAdmin);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound("Файл не найден");
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ошибка при удалении файла");
            return StatusCode(500, "Внутренняя ошибка сервера");
        }
    }
}