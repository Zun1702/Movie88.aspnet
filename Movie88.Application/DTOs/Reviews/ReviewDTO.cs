namespace Movie88.Application.DTOs.Reviews;

public class ReviewDTO
{
    public int Reviewid { get; set; }
    public int Customerid { get; set; }
    public int Movieid { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime? Createdat { get; set; }
    
    public CustomerInfoDTO? Customer { get; set; }
}

public class CustomerInfoDTO
{
    public int Customerid { get; set; }
    public string? Fullname { get; set; }
    public string? Gender { get; set; }
}

public class ReviewsPagedResultDTO
{
    public List<ReviewDTO> Reviews { get; set; } = new();
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasPrevious { get; set; }
    public bool HasNext { get; set; }
    public decimal? AverageRating { get; set; }
}
