using System.ComponentModel.DataAnnotations;

namespace Domain.DTOs;

public class GetCartItemDTO
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public List<string>? ProductImages { get; set; }
    public decimal Price { get; set; }
    public int QuantityInCart { get; set; }

    public bool IsAvailable { get; set; } 
    public string? StatusMessage { get; set; }
}

public class CartItemDetailsDTO
{
    public int TotalCount { get; set; }
    public decimal TotalPrice { get; set; }
}

public class CartItemDTO
{
    [Required(ErrorMessage = "Required_Id")]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Invalid_Quantity_Input")]
    public int Quantity { get; set; }
}