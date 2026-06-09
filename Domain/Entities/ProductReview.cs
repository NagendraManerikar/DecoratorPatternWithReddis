namespace Domain.Entities;

public class ProductReview
{
    public int ProductId { get; set; }
    public int CustomerId { get; set; }
    public string Name { get; set; }
    public string Comments { get; set; } = "";
    public double Rating { get; set; }
}