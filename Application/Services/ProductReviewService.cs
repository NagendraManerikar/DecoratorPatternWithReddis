using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services;

public class ProductReviewService
{
    private readonly IProductReviewRepository _repository;

    public ProductReviewService(
        IProductReviewRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<ProductReview>?> GetProductReviewAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task DeleteProductReviewAsync(int id)
    {
        await _repository.RemoveAsync(id);
    }
}