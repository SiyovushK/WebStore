namespace Domain.Entities;

public class Favourite
{
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid ProductId { get; set; }
    public Product? Product { get; set; }
    
    public DateTime LikedAt { get; set; } = DateTime.UtcNow;
}
