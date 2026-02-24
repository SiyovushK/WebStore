namespace Domain.Entities;

public class Order
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = "Pending"; // Pending, Paid, Shipped, Cancelled
    public string ShippingAddress { get; set; } = string.Empty;

    // Список товаров в этом заказе
    public List<OrderItem> Items { get; set; } = new();
}
