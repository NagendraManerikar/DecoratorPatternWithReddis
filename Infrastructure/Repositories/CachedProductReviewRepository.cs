using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

public class CachedProductReviewRepository : IProductReviewRepository
{
    private readonly ProductReviewRepository _repository;
    private readonly ICacheService _cacheService;

    public CachedProductReviewRepository(
        ProductReviewRepository repository,
        ICacheService cacheService)
    {
        _repository = repository;
        _cacheService = cacheService;
    }

    public async Task<List<ProductReview>?> GetByIdAsync(int id)
    {
        string cacheKey = $"product:{id}";

        // 1. Try cache
        var cachedProductReview =
            await _cacheService.GetAsync<List<ProductReview>>(cacheKey);

        if (cachedProductReview != null)
        {
            return cachedProductReview;
        }

        // 2. Fetch from DB
        var productReview =
            await _repository.GetByIdAsync(id);

        if (productReview == null)
        {
            return null;
        }

        // 3. Store in cache
        await _cacheService.SetAsync(
            cacheKey,
            productReview,
            TimeSpan.FromMinutes(5));

        return productReview;
    }

    public async Task RemoveAsync(int id)
    {
        string cacheKey = $"product:{id}";
        // 2. Remove from cache
        await _cacheService.RemoveAsync(cacheKey);
    }
}