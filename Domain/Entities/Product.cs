using Domain.Constants;

namespace Domain.Entities;

public abstract class Product
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public List<string>? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public ProductCategory Category { get; set; }

    public bool IsDeleted { get; set; } = false;
    public Guid SellerId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
}
