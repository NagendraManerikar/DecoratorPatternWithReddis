using Domain.Entities;
using Domain.Interfaces;

namespace Infrastructure.Repositories;

public class ProductReviewRepository
{
    private static readonly List<ProductReview> ProductReview =
    [
        new() { ProductId = 1, CustomerId = 100, Name = "Trimmer", Rating = 4.5, Comments = "Great product!" },
        new() { ProductId = 1, CustomerId = 101, Name = "Trimmer", Rating = 4.0, Comments = "Met expectation." },
        new() { ProductId = 2, CustomerId = 200, Name = "MI Smartphone", Rating = 3.2, Comments = "Average product." },
        new() { ProductId = 2, CustomerId = 202, Name = "MI Smartphone", Rating = 4.0, Comments = "Good camera but average batter life." }
    ];

    public async Task<List<ProductReview>?> GetByIdAsync(int id)
    {
        await Task.Delay(2000);

        return ProductReview.Where(x => x.ProductId == id).ToList();
    }
}