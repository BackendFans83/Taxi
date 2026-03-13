namespace Infrastructure.dto;

public class CreateNotificationDto
{
    public int UserId { get; set; }
    public string Topic { get; set; }
    public string Text { get; set; }
}