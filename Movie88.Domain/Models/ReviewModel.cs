namespace Movie88.Domain.Models;

public class ReviewModel
{
    public int Reviewid { get; set; }
    public int Customerid { get; set; }
    public int Movieid { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime? Createdat { get; set; }

    // Navigation properties
    public CustomerModel? Customer { get; set; }
    public MovieModel? Movie { get; set; }
}
