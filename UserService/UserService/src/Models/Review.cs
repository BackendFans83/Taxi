namespace UserService.Models;

public class Review
{
    public int Id { get; set; }
    public int AuthorId { get; set; }
    public int RecipientId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime Date { get; set; }
}
