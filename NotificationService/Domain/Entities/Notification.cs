using Domain.Enums;
namespace Infrastructure.Entities;

public class Notification
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public NotificationTopics NotificationTopic { get; set; }
    public string Text { get; set; }
}