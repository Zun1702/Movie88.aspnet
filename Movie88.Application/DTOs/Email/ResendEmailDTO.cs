namespace Movie88.Application.DTOs.Email;

public class ResendEmailRequest
{
    public string From { get; set; } = "Movie88 <movie88@ezyfix.site>";
    public List<string> To { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string Html { get; set; } = string.Empty;
}

public class ResendEmailResponse
{
    public string? Id { get; set; }
    public string? Message { get; set; }
}
