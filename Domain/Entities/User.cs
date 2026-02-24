namespace Domain.Entities;

public class User
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
    public string PasswordHash { get; set; }

    public List<CartItem> CartItems { get; set; } = new();
    public List<Favourite> Favourites { get; set; } = new();
    public List<Order> Orders { get; set; } = new();

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiry { get; set; }
}
