namespace OnlineStore.Core.DTOs
{
    // Универсальный результат для пагинации
    public class PagedResultDto<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage => PageNumber < TotalPages;
        public bool HasPreviousPage => PageNumber > 1;
    }

    // Результат для bulk операций
    public class BulkOperationResultDto<T>
    {
        public int ItemId { get; set; }
        public bool Success { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Item { get; set; }
    }
}
