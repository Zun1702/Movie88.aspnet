namespace Movie88.Application.DTOs.Common;

/// <summary>
/// Generic paged result DTO
/// Used for pagination in list endpoints
/// </summary>
public class PagedResultDTO<T>
{
    public List<T> Items { get; set; } = new();
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalItems { get; set; }
    public bool HasNextPage { get; set; }
    public bool HasPreviousPage { get; set; }
}
