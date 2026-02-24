using System.ComponentModel.DataAnnotations;
using Domain.Constants;

namespace Domain.DTOs;

public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public decimal Price { get; set; }
    public List<string>? ImageUrl { get; set; }
    public int StockQuantity { get; set; }
    public ProductCategory Category { get; set; } 
    public Guid SellerId { get; set; }

    public bool? InCart { get; set; }
    public bool? InFavourite { get; set; }

    // Поля для Книг
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public int? Pages { get; set; }

    // Поля для Phone)
    public double? ScreenSize { get; set; }
    public string? Processor { get; set; }
    public int? StorageGB { get; set; }
    public int? BatteryMah { get; set; }
    public string? OS { get; set; }

    // Поля для Одежды
    public string? Size { get; set; }
    public string? Material { get; set; }
    public string? Brand { get; set; }
}

public class ProductFilterDto
{
    public string? Search { get; set; }
    public ProductCategory? Category { get; set; }
    public Guid? SellerId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public ProductSortOption SortBy { get; set; } = ProductSortOption.CreatedAtDesc;
    
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}

public class CreateProductDto
{
    [Required(ErrorMessage = "Ivalid_Name_Input")]
    [MaxLength(100, ErrorMessage = "Ivalid_Name_Input")]
    public string Name { get; set; }

    [Required(ErrorMessage = "Ivalid_Description_Input")]
    [MaxLength(1000, ErrorMessage = "Ivalid_Description_Input")] 
    public string Description { get; set; }

    [Range(0.01, int.MaxValue, ErrorMessage = "Ivalid_Price_Input")] 
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Ivalid_StockQuantity_Input")] 
    public int StockQuantity { get; set; }

    [Required(ErrorMessage = "Ivalid_Category_Input")] 
    public ProductCategory Category { get; set; }
    
    public List<string>? ImageUrl { get; set; }

    // Поля для Книг
    public string? Author { get; set; }
    public string? ISBN { get; set; }
    public int? Pages { get; set; } 

    // Поля для Phone
    public double? ScreenSize { get; set; }
    public string? Processor { get; set; }
    public int? StorageGB { get; set; }
    public int? BatteryMah { get; set; }
    public string? OS { get; set; } 

    // Поля для Одежды
    public string? Size { get; set; }
    public string? Material { get; set; }
    public string? Brand { get; set; }
}

public class UpdateProductDto : CreateProductDto { }