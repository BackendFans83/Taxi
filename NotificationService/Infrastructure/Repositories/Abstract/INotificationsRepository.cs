using Infrastructure.dto;
using Infrastructure.Entities;

namespace Infrastructure.Repositories.Abstract;

public interface INotificationsRepository
{
    Task AddNotificationAsync(CreateNotificationDto dto);
    IEnumerable<Notification> GetNotificationsByUserIdAsync(int userId);
};