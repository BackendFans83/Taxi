using UserService.Repositories;

namespace UserService.Services;

public class ReviewService(IReviewRepository reviewRepository) : IReviewService
{

}
