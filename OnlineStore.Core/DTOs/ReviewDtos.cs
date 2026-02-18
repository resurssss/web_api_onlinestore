namespace OnlineStore.Core.DTOs;

public class ReviewCreateDto
{
    public int ProductId { get; set; }
    public int? UserId { get; set; } // Добавляем UserId для установки из Claims
    public string Author { get; set; } = "Гость";
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
}

public class ReviewUpdateDto
{
    public int? Rating { get; set; }
    public string? Comment { get; set; }
}

public class ReviewResponseDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int? UserId { get; set; } // Добавляем UserId в ответ
    public string Author { get; set; } = "Гость";
    public int Rating { get; set; }
    public string Comment { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class ReviewListItemDto
{
    public int Id { get; set; }
    public int? UserId { get; set; } // Добавляем UserId в список
    public string Author { get; set; } = "Гость";
    public int Rating { get; set; }
    public string CommentPreview { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}