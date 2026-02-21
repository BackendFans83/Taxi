using Microsoft.EntityFrameworkCore;
using UserService.Models;

namespace UserService.Repositories;

public class ReviewRepository(DbSet<Review> reviews) : IReviewRepository
{
    
}
