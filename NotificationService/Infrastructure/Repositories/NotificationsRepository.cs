using Infrastructure.dto;
using Infrastructure.Entities;

namespace Infrastructure.Repositories;

public class NotificationsRepository(NotificationsDbContext db)
{
    public async Task AddNotificationAsync(CreateNotificationDto dto)
    {
        var notification = new Notification
        {
            NotificationTopic = dto.Topic,
            Text = dto.Text,
            UserId = dto.UserId
        };
        db.Notifications.Add(notification);
        await db.SaveChangesAsync();
    }

    public IEnumerable<Notification> GetNotificationsByUserIdAsync(int userId)
    {
        return db.Notifications
            .Where(n => n.UserId == userId);
    }
}