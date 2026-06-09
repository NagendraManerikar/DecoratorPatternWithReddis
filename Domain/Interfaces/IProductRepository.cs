using Domain.Entities;
namespace Domain.Interfaces;

public interface IProductReviewRepository
{
    Task<List<ProductReview>?> GetByIdAsync(int id);
    Task RemoveAsync(int id);
}