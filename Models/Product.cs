
namespace TurboHub.Models;

public class Product : BaseEntity
{
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    [MaxLength(100)]
    public string? SKU { get; set; }

    [MaxLength(100)]
    public string? Brand { get; set; }

    [Range(0, 9999999)]
    public decimal Price { get; set; }

    public int StockQuantity { get; set; } = 0;

    public int LowStockThreshold { get; set; } = 5;

    public string? Description { get; set; }

    public string? MainImagePath { get; set; }

    public string? MainImageUrl { get; set; }

    public bool IsActive { get; set; } = true;      // Admin can show/hide product
    public bool IsFeatured { get; set; } = false;   // Show in homepage featured section
    public decimal? DiscountPrice { get; set; }     // Optional sale price
    public int ViewCount { get; set; } = 0;         // Track popularity

    // Condition of the product (e.g., New, Used, Refurbished) the default value is New
    public ProductCondition Condition { get; set; } = ProductCondition.New;

    [MaxLength(50)]
    public string? WarrantyPeriod { get; set; }

    // FK
    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    // Navigation
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
    public ICollection<ProductCompatibility> Compatibilities { get; set; } = new List<ProductCompatibility>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}